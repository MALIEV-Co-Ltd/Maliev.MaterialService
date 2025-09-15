using Asp.Versioning;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace Maliev.MaterialService.Api.Controllers;

[ApiController]
[Route("materials/v{version:apiVersion}/groups")]
[ApiVersion("1.0")]
[EnableRateLimiting("MaterialsPolicy")]
[Authorize]
public class MaterialGroupsController : ControllerBase
{
    private readonly IMaterialGroupService _materialGroupService;
    private readonly IMaterialGroupMappingService _mappingService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MaterialGroupsController> _logger;
    private readonly CacheOptions _cacheOptions;

    public MaterialGroupsController(
        IMaterialGroupService materialGroupService,
        IMaterialGroupMappingService mappingService,
        IMemoryCache cache,
        ILogger<MaterialGroupsController> logger,
        CacheOptions cacheOptions)
    {
        _materialGroupService = materialGroupService;
        _mappingService = mappingService;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting all material groups, Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        // If default pagination parameters, return all groups (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            // Check cache first
            const string cacheKey = "material_groups_all";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<MaterialGroupDto>? cachedGroups))
            {
                _logger.LogDebug("Retrieved {Count} material groups from cache", cachedGroups!.Count());
                return Ok(cachedGroups!);
            }

            var groups = await _materialGroupService.GetAllGroupsAsync();
            var allGroupDtos = _mappingService.MapToDtos(groups);

            // Cache the result
            _cache.Set(cacheKey, allGroupDtos, _cacheOptions.DefaultExpiration);
            _logger.LogDebug("Retrieved and cached {Count} material groups", allGroupDtos.Count());

            return Ok(allGroupDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _materialGroupService.GetAllGroupsPagedAsync(pagination);
        var pagedGroupDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} material groups for page {PageNumber}", pagedGroupDtos.Count(), pageNumber);

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

        return Ok(pagedGroupDtos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaterialGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialGroupDto>> GetById(int id)
    {
        _logger.LogDebug("Getting material group by ID: {Id}", id);

        // Check cache first
        var cacheKey = $"material_group_{id}";
        if (_cache.TryGetValue(cacheKey, out MaterialGroupDto? cachedGroup))
        {
            _logger.LogDebug("Retrieved material group {Id} from cache", id);
            return Ok(cachedGroup!);
        }

        var group = await _materialGroupService.GetGroupByIdAsync(id);

        if (group == null)
        {
            _logger.LogWarning("Material group not found with ID: {Id}", id);
            return NotFound($"Material group with ID {id} not found");
        }

        var groupDto = _mappingService.MapToDto(group);

        // Cache the result
        _cache.Set(cacheKey, groupDto, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved and cached material group {Id}", id);

        return Ok(groupDto);
    }

    [HttpGet("family/{familyId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetByFamily(
        int familyId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting material groups by family ID: {FamilyId}, Page: {PageNumber}, Size: {PageSize}", familyId, pageNumber, pageSize);

        // If default pagination parameters, return all groups (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            // Check cache first
            var cacheKey = $"material_groups_family_{familyId}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<MaterialGroupDto>? cachedGroups))
            {
                _logger.LogDebug("Retrieved {Count} material groups for family {FamilyId} from cache", cachedGroups!.Count(), familyId);
                return Ok(cachedGroups!);
            }

            var groups = await _materialGroupService.GetGroupsByFamilyIdAsync(familyId);
            var allGroupDtos = _mappingService.MapToDtos(groups);

            // Cache the result
            _cache.Set(cacheKey, allGroupDtos, _cacheOptions.DefaultExpiration);
            _logger.LogDebug("Retrieved and cached {Count} material groups for family {FamilyId}", allGroupDtos.Count(), familyId);

            return Ok(allGroupDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _materialGroupService.GetGroupsByFamilyIdPagedAsync(familyId, pagination);
        var pagedGroupDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} material groups for family {FamilyId} on page {PageNumber}", pagedGroupDtos.Count(), familyId, pageNumber);

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

        return Ok(pagedGroupDtos);
    }
}