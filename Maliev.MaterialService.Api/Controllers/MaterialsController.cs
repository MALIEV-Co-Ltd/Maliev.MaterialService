using Asp.Versioning;
using Maliev.MaterialService.Api.Helpers;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Maliev.MaterialService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace Maliev.MaterialService.Api.Controllers;

[ApiController]
[Route("materials/v{version:apiVersion}")]
[ApiVersion("1.0")]
[EnableRateLimiting("MaterialsPolicy")]
[Authorize] // Require valid JWT token for all endpoints
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly IMaterialSearchService _searchService;
    private readonly IMaterialMappingService _mappingService;
    private readonly ILogger<MaterialsController> _logger;

    public MaterialsController(
        IMaterialService materialService,
        IMaterialSearchService searchService,
        IMaterialMappingService mappingService,
        ILogger<MaterialsController> logger)
    {
        _materialService = materialService;
        _searchService = searchService;
        _mappingService = mappingService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting all materials, includeInactive: {IncludeInactive}, Page: {PageNumber}, Size: {PageSize}", 
            includeInactive, pageNumber, pageSize);

        // If default pagination parameters, return all materials (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            var materials = await _materialService.GetAllMaterialsAsync(includeInactive);
            var allMaterialDtos = _mappingService.MapToDtos(materials);

            _logger.LogDebug("Retrieved {Count} materials", allMaterialDtos.Count());
            return Ok(allMaterialDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _materialService.GetAllMaterialsPagedAsync(pagination, includeInactive);
        var pagedMaterialDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} materials for page {PageNumber}", pagedMaterialDtos.Count(), pageNumber);

        // Add pagination headers
        PaginationHelper.AddPaginationHeaders(Response, pagedResult);

        return Ok(pagedMaterialDtos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialDto>> GetById(int id)
    {
        _logger.LogDebug("Getting material by ID: {Id}", id);

        var material = await _materialService.GetMaterialByIdAsync(id);

        if (material == null)
        {
            _logger.LogWarning("Material not found with ID: {Id}", id);
            return NotFound($"Material with ID {id} not found");
        }

        var materialDto = _mappingService.MapToDetailedDto(material);
        return Ok(materialDto);
    }

    [HttpGet("group/{groupId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByGroupId(
        int groupId, 
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting materials by group ID: {GroupId}, includeInactive: {IncludeInactive}, Page: {PageNumber}, Size: {PageSize}", 
            groupId, includeInactive, pageNumber, pageSize);

        // If default pagination parameters, return all materials (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            var materials = await _materialService.GetMaterialsByGroupIdAsync(groupId, includeInactive);
            var materialDtos = _mappingService.MapToDtos(materials);

            _logger.LogDebug("Retrieved {Count} materials for group {GroupId}", materialDtos.Count(), groupId);
            return Ok(materialDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _materialService.GetMaterialsByGroupIdPagedAsync(groupId, pagination, includeInactive);
        var pagedMaterialDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} materials for group {GroupId} on page {PageNumber}", 
            pagedMaterialDtos.Count(), groupId, pageNumber);

        // Add pagination headers
        PaginationHelper.AddPaginationHeaders(Response, pagedResult);

        return Ok(pagedMaterialDtos);
    }

    [HttpGet("family/{familyId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByFamilyId(
        int familyId, 
        [FromQuery] bool includeInactive = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogDebug("Getting materials by family ID: {FamilyId}, includeInactive: {IncludeInactive}, Page: {PageNumber}, Size: {PageSize}", 
            familyId, includeInactive, pageNumber, pageSize);

        // If default pagination parameters, return all materials (backward compatibility)
        if (pageNumber == 1 && pageSize == 20)
        {
            var materials = await _materialService.GetMaterialsByFamilyIdAsync(familyId, includeInactive);
            var materialDtos = _mappingService.MapToDtos(materials);

            _logger.LogDebug("Retrieved {Count} materials for family {FamilyId}", materialDtos.Count(), familyId);
            return Ok(materialDtos);
        }

        // Otherwise, use pagination
        var pagination = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var pagedResult = await _materialService.GetMaterialsByFamilyIdPagedAsync(familyId, pagination, includeInactive);
        var pagedMaterialDtos = _mappingService.MapToDtos(pagedResult.Items);

        _logger.LogDebug("Retrieved {Count} materials for family {FamilyId} on page {PageNumber}", 
            pagedMaterialDtos.Count(), familyId, pageNumber);

        // Add pagination headers
        PaginationHelper.AddPaginationHeaders(Response, pagedResult);

        return Ok(pagedMaterialDtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialDto>> Create([FromBody] CreateMaterialRequest request)
    {
        _logger.LogInformation("Creating new material: {Name}", request.Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var material = _mappingService.MapFromCreateRequest(request);
        var createdMaterial = await _materialService.CreateMaterialAsync(material);

        var materialDto = _mappingService.MapToDto(createdMaterial);
        _logger.LogInformation("Created material with ID: {Id}", createdMaterial.Id);

        return CreatedAtAction(nameof(GetById), new { id = createdMaterial.Id }, materialDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialDto>> Update(int id, [FromBody] UpdateMaterialRequest request)
    {
        _logger.LogInformation("Updating material ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingMaterial = await _materialService.GetMaterialByIdAsync(id);
        if (existingMaterial == null)
        {
            _logger.LogWarning("Material not found for update with ID: {Id}", id);
            return NotFound($"Material with ID {id} not found");
        }

        var material = _mappingService.MapFromUpdateRequest(request, id);
        var updatedMaterial = await _materialService.UpdateMaterialAsync(material);

        var materialDto = _mappingService.MapToDto(updatedMaterial);
        _logger.LogInformation("Updated material with ID: {Id}", id);

        return Ok(materialDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting material ID: {Id}", id);

        var existingMaterial = await _materialService.GetMaterialByIdAsync(id);
        if (existingMaterial == null)
        {
            _logger.LogWarning("Material not found for deletion with ID: {Id}", id);
            return NotFound($"Material with ID {id} not found");
        }

        await _materialService.DeleteMaterialAsync(id);

        _logger.LogInformation("Deleted material with ID: {Id}", id);
        return NoContent();
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> Search([FromQuery] string? query, [FromQuery] MaterialSearchFilters? filters)
    {
        _logger.LogDebug("Searching materials with query: {Query}", query);

        var materials = await _searchService.SearchAsync(query ?? "", filters);
        var materialDtos = _mappingService.MapToDtos(materials);

        _logger.LogDebug("Found {Count} materials matching search criteria", materialDtos.Count());
        return Ok(materialDtos);
    }

    [HttpGet("recommend")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetRecommendations([FromQuery] MaterialRecommendationRequest request)
    {
        _logger.LogDebug("Getting material recommendations for application: {Application}", request.Application);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var materials = await _searchService.GetRecommendationsAsync(request);
        var materialDtos = _mappingService.MapToDtos(materials);

        _logger.LogDebug("Found {Count} recommended materials", materialDtos.Count());
        return Ok(materialDtos);
    }
}