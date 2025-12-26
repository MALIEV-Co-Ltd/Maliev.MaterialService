using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Mapping;
using Maliev.MaterialService.Api.Services.Cache;
using Maliev.MaterialService.Api.Contracts.Messaging;
using Maliev.MessagingContracts.Generated;
using Maliev.MaterialService.Data.DbContext;
using Maliev.MaterialService.Data.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Api.Services.Materials;

/// <summary>
/// Implementation of material management service
/// </summary>
public class MaterialService : IMaterialService
{
    private readonly MaterialDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MaterialService> _logger;
    private const string CacheKeyPrefix = "material:";

    /// <summary>
    /// Initializes a new instance of MaterialService
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="cacheService">Cache service</param>
    /// <param name="publishEndpoint">MassTransit publish endpoint</param>
    /// <param name="logger">Logger instance</param>
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

        // Manual validation for properties that might be missed by model binding in bulk operations
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

        // Check if material with same code already exists
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

        // Load related entities
        await LoadRelatedEntitiesAsync(material, request.ManufacturingProcessIds, request.ColorIds, request.PostProcessingMethodIds);

        // Add mechanical properties
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

        // Publish event
        await _publishEndpoint.Publish(new MaterialCreatedEvent(
            Guid.NewGuid(),
            nameof(MaterialCreatedEvent),
            MessageType.Event,
            "1.0",
            "MaterialService",
            new List<string>(),
            Guid.NewGuid(),
            null,
            DateTimeOffset.UtcNow,
            true,
            new MaterialCreatedEventPayload(material.Id, material.Code, material.Name, material.CreatedAt)));

        // Invalidate cache
        await InvalidateCacheAsync();

        return await GetMaterialByIdAsync(material.Id) ?? throw new InvalidOperationException("Failed to retrieve created material.");
    }

    /// <inheritdoc/>
    public async Task<MaterialResponse?> GetMaterialByIdAsync(Guid id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";

        // Try to get from cache
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
            .FirstOrDefaultAsync(m => m.Id == id && m.Active);

        if (material == null)
        {
            return null;
        }

        var response = material.ToMaterialResponse();

        // Cache the result
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
                    .FirstOrDefaultAsync(m => m.Id == id && m.Active);

                if (material == null)
                {
                    return null;
                }

                // Check optimistic concurrency
                if (material.Version != request.Version)
                {
                    throw new DbUpdateConcurrencyException("The material has been modified by another user.");
                }

                // Update basic properties
                material.UpdateMaterial(request);
                material.UpdatedBy = userId;

                // Clear and reload related entities
                material.ManufacturingProcesses.Clear();
                material.AvailableColors.Clear();
                material.PostProcessingMethods.Clear();
                material.MechanicalProperties.Clear();

                await _context.SaveChangesAsync();

                await LoadRelatedEntitiesAsync(material, request.ManufacturingProcessIds, request.ColorIds, request.PostProcessingMethodIds);

                // Add mechanical properties
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
                await transaction.CommitAsync();

                _logger.LogInformation("Material updated successfully with ID: {MaterialId}", material.Id);

                // Publish event
                await _publishEndpoint.Publish(new MaterialUpdatedEvent(
                    Guid.NewGuid(),
                    nameof(MaterialUpdatedEvent),
                    MessageType.Event,
                    "1.0",
                    "MaterialService",
                    new List<string>(),
                    Guid.NewGuid(),
                    null,
                    DateTimeOffset.UtcNow,
                    true,
                    new MaterialUpdatedEventPayload(material.Id, material.Code, material.Name, material.UpdatedAt ?? DateTimeOffset.UtcNow, material.Version)));

                // Invalidate cache
                await InvalidateCacheAsync(id);

                return await GetMaterialByIdAsync(id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
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

        // Soft delete
        material.Active = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Material soft-deleted successfully with ID: {MaterialId}", material.Id);

        // Publish event
        await _publishEndpoint.Publish(new MaterialDeletedEvent(
            Guid.NewGuid(),
            nameof(MaterialDeletedEvent),
            MessageType.Event,
            "1.0",
            "MaterialService",
            new List<string>(),
            Guid.NewGuid(),
            null,
            DateTimeOffset.UtcNow,
            true,
            new MaterialDeletedEventPayload(material.Id, DateTimeOffset.UtcNow)));

        // Invalidate cache
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

        // Apply filters
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

        // Filter by color
        if (!string.IsNullOrWhiteSpace(color))
        {
            query = query.Where(m => m.AvailableColors.Any(c => c.Name == color));
        }

        // Filter by mechanical properties (Tensile Strength)
        if (minTensileStrength.HasValue || maxTensileStrength.HasValue)
        {
            query = query.Where(m => m.MechanicalProperties.Any(mp =>
                mp.MechanicalProperty.Name == "Tensile Strength" &&
                (!minTensileStrength.HasValue || mp.Value >= minTensileStrength.Value) &&
                (!maxTensileStrength.HasValue || mp.Value <= maxTensileStrength.Value)
            ));
        }

        // Include related entities
        query = query
            .Include(m => m.Supplier)
            .Include(m => m.ManufacturingProcesses)
            .Include(m => m.AvailableColors)
            .Include(m => m.PostProcessingMethods)
            .Include(m => m.MechanicalProperties)
                .ThenInclude(mp => mp.MechanicalProperty);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name),
            "code" => sortDescending ? query.OrderByDescending(m => m.Code) : query.OrderBy(m => m.Code),
            "price" => sortDescending ? query.OrderByDescending(m => m.PricePerUnit) : query.OrderBy(m => m.PricePerUnit),
            "stock" => sortDescending ? query.OrderByDescending(m => m.StockLevel) : query.OrderBy(m => m.StockLevel),
            "createdat" => sortDescending ? query.OrderByDescending(m => m.CreatedAt) : query.OrderBy(m => m.CreatedAt),
            _ => query.OrderBy(m => m.Name) // Default sorting
        };

        // Apply pagination
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
                .Where(mp => manufacturingProcessIds.Contains(mp.Id))
                .ToListAsync();
            foreach (var process in processes)
            {
                material.ManufacturingProcesses.Add(process);
            }
        }

        if (colorIds != null && colorIds.Any())
        {
            var colors = await _context.Colors
                .Where(c => colorIds.Contains(c.Id))
                .ToListAsync();
            foreach (var color in colors)
            {
                material.AvailableColors.Add(color);
            }
        }

        if (postProcessingMethodIds != null && postProcessingMethodIds.Any())
        {
            var methods = await _context.PostProcessingMethods
                .Where(ppm => postProcessingMethodIds.Contains(ppm.Id))
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
            // For now, we just log. In a real implementation, you might have a pattern-based cache invalidation
            _logger.LogDebug("Cache invalidation requested for all materials");
        }
    }
}
