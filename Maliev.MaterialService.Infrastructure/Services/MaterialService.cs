using Maliev.Aspire.ServiceDefaults.Caching;
using Maliev.MaterialService.Application.DTOs;
using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Application.Services;
using Maliev.MaterialService.Domain.Entities;
using Maliev.MaterialService.Infrastructure.Mapping;
using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Infrastructure.Search;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Materials;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Infrastructure.Services;

/// <summary>
/// Implementation of material management service.
/// </summary>
public class MaterialService : IMaterialService
{
    private readonly MaterialDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MaterialService> _logger;
    private const string CacheKeyPrefix = "material:";

    /// <summary>
    /// Initializes a new instance of MaterialService.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="cacheService">Cache service.</param>
    /// <param name="publishEndpoint">MassTransit publish endpoint.</param>
    /// <param name="logger">Logger instance.</param>
    public MaterialService(
        MaterialDbContext context,
        ICacheService cacheService,
        IPublishEndpoint publishEndpoint,
        ILogger<MaterialService> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<MaterialResponse> CreateMaterialAsync(CreateMaterialRequest request, string userId)
    {
        _logger.LogInformation("Creating new material with code: {Code}", request.Code);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Material name is required.");
        }
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Material code is required.");
        }
        if (request.StockLevel < 0)
        {
            throw new InvalidOperationException("Stock level cannot be negative.");
        }
        if (request.PricePerUnit <= 0)
        {
            throw new InvalidOperationException("Price per unit must be greater than zero.");
        }

        var existingMaterial = await _context.Materials
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Code == request.Code);

        if (existingMaterial != null)
        {
            throw new InvalidOperationException($"Material with code '{request.Code}' already exists.");
        }

        var material = request.ToMaterial();
        material.Id = Guid.NewGuid();
        material.CreatedBy = userId;
        material.Active = true;

        await LoadRelatedEntitiesAsync(material, request.ManufacturingProcessIds, request.ColorIds, request.PostProcessingMethodIds);

        foreach (var prop in request.MechanicalProperties)
        {
            material.MechanicalProperties.Add(new MaterialMechanicalProperty
            {
                MaterialId = material.Id,
                MechanicalPropertyId = prop.MechanicalPropertyId,
                Value = prop.Value
            });
        }

        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Material created successfully with ID: {MaterialId}", material.Id);

        await _publishEndpoint.Publish(new MaterialCreatedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: "MaterialCreatedEvent",
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: "MaterialService",
            ConsumedBy: ["InventoryService", "NotificationService"],
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new MaterialCreatedEventPayload(
                MaterialId: material.Id,
                Code: material.Code,
                Name: material.Name,
                PricePerUnit: (double)material.PricePerUnit,
                StockLevel: material.StockLevel,
                CreatedAt: material.CreatedAt
            )
        ));

        _logger.LogInformation("Published MaterialCreatedEvent for material {MaterialId}", material.Id);

        await _publishEndpoint.Publish(
            MaterialSearchDocumentMapper.ToUpsertEvent(material, DateTimeOffset.UtcNow));

        await InvalidateCacheAsync();

        return await GetMaterialByIdAsync(material.Id) ?? throw new InvalidOperationException("Failed to retrieve created material.");
    }

    /// <inheritdoc/>
    public async Task<MaterialResponse?> GetMaterialByIdAsync(Guid id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        var cachedMaterial = await _cacheService.GetAsync<MaterialResponse>(cacheKey);
        if (cachedMaterial != null)
        {
            _logger.LogDebug("Material {MaterialId} retrieved from cache", id);
            return cachedMaterial;
        }

        var material = await _context.Materials
            .AsNoTracking()
            .Include(m => m.Supplier)
            .Include(m => m.ManufacturingProcesses)
            .Include(m => m.AvailableColors)
            .Include(m => m.PostProcessingMethods)
            .Include(m => m.MechanicalProperties)
                .ThenInclude(mp => mp.MechanicalProperty)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id && m.Active);

        if (material == null)
        {
            return null;
        }

        var response = material.ToMaterialResponse();

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

        return response;
    }

    /// <inheritdoc/>
    public async Task<MaterialResponse?> UpdateMaterialAsync(Guid id, UpdateMaterialRequest request, string userId)
    {
        _logger.LogInformation("Updating material with ID: {MaterialId}", id);

        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var material = await _context.Materials
                    .Include(m => m.ManufacturingProcesses)
                    .Include(m => m.AvailableColors)
                    .Include(m => m.PostProcessingMethods)
                    .Include(m => m.MechanicalProperties)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(m => m.Id == id && m.Active);

                if (material == null)
                {
                    return null;
                }

                material.UpdateMaterial(request);
                material.UpdatedBy = userId;

                material.ManufacturingProcesses.Clear();
                material.AvailableColors.Clear();
                material.PostProcessingMethods.Clear();
                material.MechanicalProperties.Clear();

                await LoadRelatedEntitiesAsync(material, request.ManufacturingProcessIds, request.ColorIds, request.PostProcessingMethodIds);

                foreach (var prop in request.MechanicalProperties)
                {
                    material.MechanicalProperties.Add(new MaterialMechanicalProperty
                    {
                        MaterialId = material.Id,
                        MechanicalPropertyId = prop.MechanicalPropertyId,
                        Value = prop.Value
                    });
                }

                await _context.SaveChangesAsync();

                var version = (int)_context.Entry(material).Property<uint>("xmin").CurrentValue;

                _logger.LogInformation("Material updated successfully with ID: {MaterialId}", material.Id);

                await _publishEndpoint.Publish(new MaterialUpdatedEvent(
                    MessageId: Guid.NewGuid(),
                    MessageName: "MaterialUpdatedEvent",
                    MessageType: MessageType.Event,
                    MessageVersion: "1.0.0",
                    PublishedBy: "MaterialService",
                    ConsumedBy: ["InventoryService", "NotificationService"],
                    CorrelationId: Guid.NewGuid(),
                    CausationId: null,
                    OccurredAtUtc: DateTimeOffset.UtcNow,
                    IsPublic: false,
                    Payload: new MaterialUpdatedEventPayload(
                        MaterialId: material.Id,
                        Code: material.Code,
                        Name: material.Name,
                        UpdatedAt: material.UpdatedAt ?? DateTimeOffset.UtcNow,
                        Version: version
                    )
                ));

                _logger.LogInformation("Published MaterialUpdatedEvent for material {MaterialId}", material.Id);

                await _publishEndpoint.Publish(
                    MaterialSearchDocumentMapper.ToUpsertEvent(material, DateTimeOffset.UtcNow));

                await transaction.CommitAsync();

                await InvalidateCacheAsync(id);

                return await GetMaterialByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating material {MaterialId}", id);
                if (transaction.TransactionId != Guid.Empty)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rbEx)
                    {
                        _logger.LogError(rbEx, "Failed to rollback transaction");
                    }
                }
                throw;
            }
        });
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteMaterialAsync(Guid id)
    {
        _logger.LogInformation("Deleting material with ID: {MaterialId}", id);

        var material = await _context.Materials.FirstOrDefaultAsync(m => m.Id == id && m.Active);

        if (material == null)
        {
            return false;
        }

        material.Active = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Material soft-deleted successfully with ID: {MaterialId}", material.Id);

        await _publishEndpoint.Publish(new MaterialDiscontinuedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: "MaterialDiscontinuedEvent",
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: "MaterialService",
            ConsumedBy: ["InventoryService", "NotificationService"],
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: DateTimeOffset.UtcNow,
            IsPublic: false,
            Payload: new MaterialDiscontinuedEventPayload(
                MaterialId: material.Id,
                DiscontinuedAt: DateTimeOffset.UtcNow
            )
        ));

        _logger.LogInformation("Published MaterialDiscontinuedEvent for material {MaterialId}", material.Id);

        await _publishEndpoint.Publish(
            MaterialSearchDocumentMapper.ToDeletedEvent(material.Id, DateTimeOffset.UtcNow));

        await InvalidateCacheAsync(id);

        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MaterialResponse>> GetAllMaterialsAsync()
    {
        var materials = await _context.Materials
            .AsNoTracking()
            .Include(m => m.Supplier)
            .Include(m => m.ManufacturingProcesses)
            .Include(m => m.AvailableColors)
            .Include(m => m.PostProcessingMethods)
            .Include(m => m.MechanicalProperties)
                .ThenInclude(mp => mp.MechanicalProperty)
            .Where(m => m.Active)
            .AsSplitQuery()
            .ToListAsync();

        return materials.Select(m => m.ToMaterialResponse());
    }

    /// <inheritdoc/>
    public async Task<PagedResult<MaterialResponse>> GetMaterialsAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Guid? supplierId = null,
        string? manufacturingProcess = null,
        string? color = null,
        decimal? minTensileStrength = null,
        decimal? maxTensileStrength = null)
    {
        _logger.LogInformation("Getting materials with filters: page={Page}, pageSize={PageSize}, search={SearchTerm}, " +
                               "manufacturingProcess={ManufacturingProcess}, color={Color}, " +
                               "minTensileStrength={MinTensileStrength}, maxTensileStrength={MaxTensileStrength}",
            page, pageSize, searchTerm, manufacturingProcess, color, minTensileStrength, maxTensileStrength);

        var query = _context.Materials
            .AsNoTracking()
            .Where(m => m.Active);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m =>
                m.Name.Contains(searchTerm) ||
                m.Code.Contains(searchTerm) ||
                (m.Description != null && m.Description.Contains(searchTerm)));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(m => m.PricePerUnit >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(m => m.PricePerUnit <= maxPrice.Value);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(m => m.SupplierId == supplierId.Value);
        }

        _logger.LogDebug("Applying manufacturing process filter: {ManufacturingProcess}", manufacturingProcess);
        if (!string.IsNullOrWhiteSpace(manufacturingProcess))
        {
            query = query.Where(m => m.ManufacturingProcesses.Any(mp => mp.Name == manufacturingProcess));
        }

        if (!string.IsNullOrWhiteSpace(color))
        {
            query = query.Where(m => m.AvailableColors.Any(c => c.Name == color));
        }

        if (minTensileStrength.HasValue || maxTensileStrength.HasValue)
        {
            query = query.Where(m => m.MechanicalProperties.Any(mp =>
                mp.MechanicalProperty.Active &&
                mp.MechanicalProperty.Name.ToLower() == "tensile strength" &&
                (!minTensileStrength.HasValue || mp.Value >= minTensileStrength.Value) &&
                (!maxTensileStrength.HasValue || mp.Value <= maxTensileStrength.Value)
            ));
        }

        var totalCount = await query.CountAsync();

        query = query
            .Include(m => m.Supplier)
            .Include(m => m.ManufacturingProcesses.Where(mp => mp.Active))
            .Include(m => m.AvailableColors.Where(c => c.Active))
            .Include(m => m.PostProcessingMethods.Where(ppm => ppm.Active))
            .Include(m => m.MechanicalProperties.Where(mp => mp.MechanicalProperty.Active))
                .ThenInclude(mp => mp.MechanicalProperty)
            .AsSplitQuery();

        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name),
            "code" => sortDescending ? query.OrderByDescending(m => m.Code) : query.OrderBy(m => m.Code),
            "price" => sortDescending ? query.OrderByDescending(m => m.PricePerUnit) : query.OrderBy(m => m.PricePerUnit),
            "stock" => sortDescending ? query.OrderByDescending(m => m.StockLevel) : query.OrderBy(m => m.StockLevel),
            "createdat" => sortDescending ? query.OrderByDescending(m => m.CreatedAt) : query.OrderBy(m => m.CreatedAt),
            _ => query.OrderBy(m => m.Name)
        };

        var materials = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var responses = materials.Select(m => m.ToMaterialResponse());

        return new PagedResult<MaterialResponse>
        {
            Items = responses,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <inheritdoc/>
    public async Task<int> GetSupplierReferenceCountAsync(Guid supplierId)
    {
        _logger.LogInformation("Checking material references for supplier {SupplierId}", supplierId);

        var count = await _context.Materials
            .AsNoTracking()
            .Where(m => m.Active && m.SupplierId == supplierId)
            .CountAsync();

        _logger.LogInformation("Found {Count} materials referencing supplier {SupplierId}", count, supplierId);

        return count;
    }

    private async Task LoadRelatedEntitiesAsync(
        Material material,
        List<Guid> manufacturingProcessIds,
        List<Guid> colorIds,
        List<Guid> postProcessingMethodIds)
    {
        if (manufacturingProcessIds != null && manufacturingProcessIds.Any())
        {
            var processes = await _context.ManufacturingProcesses
                .Where(mp => mp.Active && manufacturingProcessIds.Contains(mp.Id))
                .ToListAsync();
            foreach (var process in processes)
            {
                material.ManufacturingProcesses.Add(process);
            }
        }

        if (colorIds != null && colorIds.Any())
        {
            var colors = await _context.Colors
                .Where(c => c.Active && colorIds.Contains(c.Id))
                .ToListAsync();
            foreach (var color in colors)
            {
                material.AvailableColors.Add(color);
            }
        }

        if (postProcessingMethodIds != null && postProcessingMethodIds.Any())
        {
            var methods = await _context.PostProcessingMethods
                .Where(ppm => ppm.Active && postProcessingMethodIds.Contains(ppm.Id))
                .ToListAsync();
            foreach (var method in methods)
            {
                material.PostProcessingMethods.Add(method);
            }
        }
    }

    private async Task InvalidateCacheAsync(Guid? materialId = null)
    {
        if (materialId.HasValue)
        {
            var cacheKey = $"{CacheKeyPrefix}{materialId.Value}";
            await _cacheService.RemoveAsync(cacheKey);
            _logger.LogDebug("Cache invalidated for material {MaterialId}", materialId.Value);
        }
        else
        {
            _logger.LogDebug("Cache invalidation requested for all materials");
        }
    }
}
