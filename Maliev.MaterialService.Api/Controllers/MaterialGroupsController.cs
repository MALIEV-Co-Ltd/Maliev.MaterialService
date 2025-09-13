using Asp.Versioning;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Maliev.MaterialService.Api.Controllers;

[ApiController]
[Route("materials/v{version:apiVersion}/groups")]
[ApiVersion("1.0")]
[EnableRateLimiting("MaterialsPolicy")]
[Authorize]
public class MaterialGroupsController : ControllerBase
{
    private readonly IMaterialGroupService _materialGroupService;
    private readonly ILogger<MaterialGroupsController> _logger;

    public MaterialGroupsController(
        IMaterialGroupService materialGroupService,
        ILogger<MaterialGroupsController> logger)
    {
        _materialGroupService = materialGroupService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetAll()
    {
        _logger.LogDebug("Getting all material groups");

        var groups = await _materialGroupService.GetAllGroupsAsync();
        var groupDtos = groups.Select(g => new MaterialGroupDto
        {
            Id = g.Id,
            MaterialFamilyId = g.MaterialFamilyId,
            Name = g.Name,
            Description = g.Description,
            SortOrder = g.SortOrder,
            MaterialFamily = g.MaterialFamily != null ? new MaterialFamilyDto
            {
                Id = g.MaterialFamily.Id,
                Name = g.MaterialFamily.Name,
                Description = g.MaterialFamily.Description,
                SortOrder = g.MaterialFamily.SortOrder
            } : null
        });

        _logger.LogDebug("Retrieved {Count} material groups", groupDtos.Count());
        return Ok(groupDtos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaterialGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialGroupDto>> GetById(int id)
    {
        _logger.LogDebug("Getting material group by ID: {Id}", id);

        var group = await _materialGroupService.GetGroupByIdAsync(id);

        if (group == null)
        {
            _logger.LogWarning("Material group not found with ID: {Id}", id);
            return NotFound($"Material group with ID {id} not found");
        }

        var groupDto = new MaterialGroupDto
        {
            Id = group.Id,
            MaterialFamilyId = group.MaterialFamilyId,
            Name = group.Name,
            Description = group.Description,
            SortOrder = group.SortOrder,
            MaterialFamily = group.MaterialFamily != null ? new MaterialFamilyDto
            {
                Id = group.MaterialFamily.Id,
                Name = group.MaterialFamily.Name,
                Description = group.MaterialFamily.Description,
                SortOrder = group.MaterialFamily.SortOrder
            } : null
        };

        return Ok(groupDto);
    }

    [HttpGet("family/{familyId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialGroupDto>>> GetByFamilyId(int familyId)
    {
        _logger.LogDebug("Getting material groups by family ID: {FamilyId}", familyId);

        var groups = await _materialGroupService.GetGroupsByFamilyIdAsync(familyId);
        var groupDtos = groups.Select(g => new MaterialGroupDto
        {
            Id = g.Id,
            MaterialFamilyId = g.MaterialFamilyId,
            Name = g.Name,
            Description = g.Description,
            SortOrder = g.SortOrder,
            MaterialFamily = g.MaterialFamily != null ? new MaterialFamilyDto
            {
                Id = g.MaterialFamily.Id,
                Name = g.MaterialFamily.Name,
                Description = g.MaterialFamily.Description,
                SortOrder = g.MaterialFamily.SortOrder
            } : null
        });

        _logger.LogDebug("Retrieved {Count} material groups for family {FamilyId}", groupDtos.Count(), familyId);
        return Ok(groupDtos);
    }
}