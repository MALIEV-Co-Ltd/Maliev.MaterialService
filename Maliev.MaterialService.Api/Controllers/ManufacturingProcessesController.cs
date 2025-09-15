using Asp.Versioning;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace Maliev.MaterialService.Api.Controllers;

[ApiController]
[Route("materials/v{version:apiVersion}/processes")]
[ApiVersion("1.0")]
[EnableRateLimiting("MaterialsPolicy")]
[Authorize]
public class ManufacturingProcessesController : ControllerBase
{
    private readonly IManufacturingProcessService _processService;
    private readonly IManufacturingProcessMappingService _mappingService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ManufacturingProcessesController> _logger;
    private readonly CacheOptions _cacheOptions;

    public ManufacturingProcessesController(
        IManufacturingProcessService processService,
        IManufacturingProcessMappingService mappingService,
        IMemoryCache cache,
        ILogger<ManufacturingProcessesController> logger,
        CacheOptions cacheOptions)
    {
        _processService = processService;
        _mappingService = mappingService;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ManufacturingProcessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<ManufacturingProcessDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting all manufacturing processes, Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // If default pagination parameters, return all processes (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            // Check cache first
            const string cacheKey = "manufacturing_processes_all";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<ManufacturingProcessDto>? cachedProcesses))
            {
                _logger.LogDebug("Retrieved {Count} manufacturing processes from cache", cachedProcesses!.Count());
                return Ok(cachedProcesses!);
            }

            var processes = await _processService.GetAllProcessesAsync();
            var allProcessDtos = _mappingService.MapToDtos(processes);

            // Cache the result
            _cache.Set(cacheKey, allProcessDtos, _cacheOptions.DefaultExpiration);
            _logger.LogDebug("Retrieved and cached {Count} manufacturing processes", allProcessDtos.Count());

            return Ok(allProcessDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _processService.GetAllProcessesPagedAsync(pagination);
        var pagedProcessDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} manufacturing processes for page {PageNumber}", pagedProcessDtos.Count(), pageNumber);

        // Add pagination headers
        Response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(new
        {
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage
        });

        return Ok(pagedProcessDtos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ManufacturingProcessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ManufacturingProcessDto>> GetById(int id)
    {
        _logger.LogDebug("Getting manufacturing process by ID: {Id}", id);

        // Check cache first
        var cacheKey = $"manufacturing_process_{id}";
        if (_cache.TryGetValue(cacheKey, out ManufacturingProcessDto? cachedProcess))
        {
            _logger.LogDebug("Retrieved manufacturing process {Id} from cache", id);
            return Ok(cachedProcess!);
        }

        var process = await _processService.GetProcessByIdAsync(id);

        if (process == null)
        {
            _logger.LogWarning("Manufacturing process not found with ID: {Id}", id);
            return NotFound($"Manufacturing process with ID {id} not found");
        }

        var processDto = _mappingService.MapToDto(process);

        // Cache the result
        _cache.Set(cacheKey, processDto, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved and cached manufacturing process {Id}", id);

        return Ok(processDto);
    }

    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ManufacturingProcessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<ManufacturingProcessDto>>> GetByCategory(
        int categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting manufacturing processes by category ID: {CategoryId}, Page: {PageNumber}, Size: {PageSize}", categoryId, pageNumber, pageSize);

        // If default pagination parameters, return all processes (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            // Check cache first
            var cacheKey = $"manufacturing_processes_category_{categoryId}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<ManufacturingProcessDto>? cachedProcesses))
            {
                _logger.LogDebug("Retrieved {Count} manufacturing processes for category {CategoryId} from cache", cachedProcesses!.Count(), categoryId);
                return Ok(cachedProcesses!);
            }

            var processes = await _processService.GetProcessesByCategoryIdAsync(categoryId);
            var allProcessDtos = _mappingService.MapToDtos(processes);

            // Cache the result
            _cache.Set(cacheKey, allProcessDtos, _cacheOptions.DefaultExpiration);
            _logger.LogDebug("Retrieved and cached {Count} manufacturing processes for category {CategoryId}", allProcessDtos.Count(), categoryId);

            return Ok(allProcessDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _processService.GetProcessesByCategoryIdPagedAsync(categoryId, pagination);
        var pagedProcessDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} manufacturing processes for category {CategoryId} on page {PageNumber}", pagedProcessDtos.Count(), categoryId, pageNumber);

        // Add pagination headers
        Response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(new
        {
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage
        });

        return Ok(pagedProcessDtos);
    }

    [HttpGet("{processId:int}/materials")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetCompatibleMaterials(int processId)
    {
        _logger.LogDebug("Getting materials compatible with process ID: {ProcessId}", processId);

        var materials = await _processService.GetCompatibleMaterialsAsync(processId);
        var materialDtos = materials.Select(m => new MaterialDto
        {
            Id = m.Id,
            MaterialGroupId = m.MaterialGroupId,
            Name = m.Name,
            Description = m.Description,
            MaterialNumber = m.MaterialNumber,
            ManufacturerReference = m.ManufacturerReference,
            DensityKilogramPerCubicMeter = m.DensityKilogramPerCubicMeter,
            TensileStrengthUltimateGigaPascal = m.TensileStrengthUltimateGigaPascal,
            TensileStrengthYieldMegaPascal = m.TensileStrengthYieldMegaPascal,
            MachinabilityPercent = m.MachinabilityPercent,
            ShearModulusGigaPascal = m.ShearModulusGigaPascal,
            ThermalConductivityWattPerMeterKelvin = m.ThermalConductivityWattPerMeterKelvin,
            PricePerKilogram = m.PricePerKilogram,
            CurrencyCode = m.CurrencyCode,
            Url = m.Url,
            Comment = m.Comment,
            IsActive = m.IsActive,
            CreatedDate = m.CreatedDate,
            ModifiedDate = m.ModifiedDate,
            MaterialGroup = m.MaterialGroup != null ? new MaterialGroupDto
            {
                Id = m.MaterialGroup.Id,
                MaterialFamilyId = m.MaterialGroup.MaterialFamilyId,
                Name = m.MaterialGroup.Name,
                Description = m.MaterialGroup.Description,
                SortOrder = m.MaterialGroup.SortOrder
            } : null
        });

        _logger.LogDebug("Retrieved {Count} materials compatible with process {ProcessId}", materialDtos.Count(), processId);
        return Ok(materialDtos);
    }
}