using Asp.Versioning;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Maliev.MaterialService.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
    private readonly ILogger<MaterialsController> _logger;

    public MaterialsController(
        IMaterialService materialService,
        IMaterialSearchService searchService,
        ILogger<MaterialsController> logger)
    {
        _materialService = materialService;
        _searchService = searchService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        _logger.LogDebug("Getting all materials, includeInactive: {IncludeInactive}", includeInactive);

        var materials = await _materialService.GetAllMaterialsAsync(includeInactive);
        var materialDtos = materials.Select(MapToDto);

        _logger.LogDebug("Retrieved {Count} materials", materialDtos.Count());
        return Ok(materialDtos);
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

        var materialDto = MapToDetailedDto(material);
        return Ok(materialDto);
    }

    [HttpGet("group/{groupId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByGroupId(int groupId, [FromQuery] bool includeInactive = false)
    {
        _logger.LogDebug("Getting materials by group ID: {GroupId}, includeInactive: {IncludeInactive}", groupId, includeInactive);

        var materials = await _materialService.GetMaterialsByGroupIdAsync(groupId, includeInactive);
        var materialDtos = materials.Select(MapToDto);

        _logger.LogDebug("Retrieved {Count} materials for group {GroupId}", materialDtos.Count(), groupId);
        return Ok(materialDtos);
    }

    [HttpGet("family/{familyId:int}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByFamilyId(int familyId, [FromQuery] bool includeInactive = false)
    {
        _logger.LogDebug("Getting materials by family ID: {FamilyId}, includeInactive: {IncludeInactive}", familyId, includeInactive);

        var materials = await _materialService.GetMaterialsByFamilyIdAsync(familyId, includeInactive);
        var materialDtos = materials.Select(MapToDto);

        _logger.LogDebug("Retrieved {Count} materials for family {FamilyId}", materialDtos.Count(), familyId);
        return Ok(materialDtos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialDto>> Create([FromBody] CreateMaterialRequest request)
    {
        _logger.LogDebug("Creating new material: {Name}", request.Name);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var material = MapFromCreateRequest(request);
        var createdMaterial = await _materialService.CreateMaterialAsync(material);

        var materialDto = MapToDto(createdMaterial);
        _logger.LogInformation("Created material with ID: {Id}", createdMaterial.Id);

        return CreatedAtAction(nameof(GetById), new { id = createdMaterial.Id }, materialDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MaterialDto>> Update(int id, [FromBody] UpdateMaterialRequest request)
    {
        _logger.LogDebug("Updating material ID: {Id}", id);

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

        var material = MapFromUpdateRequest(request, id);
        var updatedMaterial = await _materialService.UpdateMaterialAsync(material);

        var materialDto = MapToDto(updatedMaterial);
        _logger.LogInformation("Updated material with ID: {Id}", id);

        return Ok(materialDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogDebug("Deleting material ID: {Id}", id);

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
        var materialDtos = materials.Select(MapToDto);

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
        var materialDtos = materials.Select(MapToDto);

        _logger.LogDebug("Found {Count} recommended materials", materialDtos.Count());
        return Ok(materialDtos);
    }

    private static MaterialDto MapToDto(Material material)
    {
        return new MaterialDto
        {
            Id = material.Id,
            MaterialGroupId = material.MaterialGroupId,
            Name = material.Name,
            Description = material.Description,
            MaterialNumber = material.MaterialNumber,
            ManufacturerReference = material.ManufacturerReference,
            DensityKilogramPerCubicMeter = material.DensityKilogramPerCubicMeter,
            TensileStrengthUltimateGigaPascal = material.TensileStrengthUltimateGigaPascal,
            TensileStrengthYieldMegaPascal = material.TensileStrengthYieldMegaPascal,
            MachinabilityPercent = material.MachinabilityPercent,
            ShearModulusGigaPascal = material.ShearModulusGigaPascal,
            ThermalConductivityWattPerMeterKelvin = material.ThermalConductivityWattPerMeterKelvin,
            PricePerKilogram = material.PricePerKilogram,
            CurrencyId = material.CurrencyId,
            Url = material.Url,
            Comment = material.Comment,
            IsActive = material.IsActive,
            CreatedDate = material.CreatedDate,
            ModifiedDate = material.ModifiedDate,
            MaterialGroup = material.MaterialGroup != null ? new MaterialGroupDto
            {
                Id = material.MaterialGroup.Id,
                MaterialFamilyId = material.MaterialGroup.MaterialFamilyId,
                Name = material.MaterialGroup.Name,
                Description = material.MaterialGroup.Description,
                SortOrder = material.MaterialGroup.SortOrder,
                MaterialFamily = material.MaterialGroup.MaterialFamily != null ? new MaterialFamilyDto
                {
                    Id = material.MaterialGroup.MaterialFamily.Id,
                    Name = material.MaterialGroup.MaterialFamily.Name,
                    Description = material.MaterialGroup.MaterialFamily.Description,
                    SortOrder = material.MaterialGroup.MaterialFamily.SortOrder
                } : null
            } : null
        };
    }

    private static MaterialDto MapToDetailedDto(Material material)
    {
        var dto = MapToDto(material);

        // Add detailed navigation properties
        dto.MaterialStandards = material.MaterialStandards?.Select(ms => new MaterialStandardDto
        {
            Id = ms.Id,
            MaterialId = ms.MaterialId,
            StandardTypeId = ms.StandardTypeId,
            StandardValue = ms.StandardValue,
            StandardType = ms.StandardType != null ? new MaterialStandardTypeDto
            {
                Id = ms.StandardType.Id,
                Name = ms.StandardType.Name,
                Description = ms.StandardType.Description
            } : null
        }).ToList();

        dto.MaterialProperties = material.MaterialProperties?.Select(mp => new MaterialPropertyDto
        {
            Id = mp.Id,
            MaterialId = mp.MaterialId,
            PropertySubtypeId = mp.PropertySubtypeId,
            Value = mp.Value,
            PropertySubtype = mp.PropertySubtype != null ? new PropertySubtypeDto
            {
                Id = mp.PropertySubtype.Id,
                PropertyTypeId = mp.PropertySubtype.PropertyTypeId,
                Name = mp.PropertySubtype.Name,
                Description = mp.PropertySubtype.Description,
                Unit = mp.PropertySubtype.Unit,
                PropertyType = mp.PropertySubtype.PropertyType != null ? new PropertyTypeDto
                {
                    Id = mp.PropertySubtype.PropertyType.Id,
                    Name = mp.PropertySubtype.PropertyType.Name,
                    Description = mp.PropertySubtype.PropertyType.Description
                } : null
            } : null
        }).ToList();

        dto.ProcessCompatibilities = material.ProcessCompatibilities?.Select(pc => new MaterialProcessCompatibilityDto
        {
            Id = pc.Id,
            MaterialId = pc.MaterialId,
            ProcessId = pc.ProcessId,
            CompatibilityScore = pc.CompatibilityLevel,
            Notes = pc.ProcessingNotes,
            Process = pc.Process != null ? new ManufacturingProcessDto
            {
                Id = pc.Process.Id,
                CategoryId = pc.Process.CategoryId,
                Name = pc.Process.Name,
                Description = pc.Process.Description,
                SortOrder = pc.Process.SortOrder,
                Category = pc.Process.Category != null ? new ManufacturingProcessCategoryDto
                {
                    Id = pc.Process.Category.Id,
                    Name = pc.Process.Category.Name,
                    Description = pc.Process.Category.Description,
                    SortOrder = pc.Process.Category.SortOrder
                } : null
            } : null
        }).ToList();

        dto.MaterialColors = material.MaterialColors?.Select(mc => new MaterialColorDto
        {
            Id = mc.Id,
            MaterialId = mc.MaterialId,
            ColorId = mc.ColorId,
            Color = mc.Color != null ? new ColorDto
            {
                Id = mc.Color.Id,
                Name = mc.Color.Name,
                HexCode = mc.Color.HexCode,
                Description = mc.Color.Description
            } : null
        }).ToList();

        dto.MaterialSurfaceFinishes = material.MaterialSurfaceFinishes?.Select(msf => new MaterialSurfaceFinishDto
        {
            Id = msf.Id,
            MaterialId = msf.MaterialId,
            SurfaceFinishId = msf.SurfaceFinishId,
            SurfaceFinish = msf.SurfaceFinish != null ? new SurfaceFinishDto
            {
                Id = msf.SurfaceFinish.Id,
                Name = msf.SurfaceFinish.Name,
                Description = msf.SurfaceFinish.Description,
                RoughnessRa = null
            } : null
        }).ToList();

        return dto;
    }

    private static Material MapFromCreateRequest(CreateMaterialRequest request)
    {
        return new Material
        {
            MaterialGroupId = request.MaterialGroupId,
            Name = request.Name,
            Description = request.Description,
            MaterialNumber = request.MaterialNumber,
            ManufacturerReference = request.ManufacturerReference,
            DensityKilogramPerCubicMeter = request.DensityKilogramPerCubicMeter,
            TensileStrengthUltimateGigaPascal = request.TensileStrengthUltimateGigaPascal,
            TensileStrengthYieldMegaPascal = request.TensileStrengthYieldMegaPascal,
            MachinabilityPercent = request.MachinabilityPercent,
            ShearModulusGigaPascal = request.ShearModulusGigaPascal,
            ThermalConductivityWattPerMeterKelvin = request.ThermalConductivityWattPerMeterKelvin,
            PricePerKilogram = request.PricePerKilogram,
            CurrencyId = request.CurrencyId,
            Url = request.Url,
            Comment = request.Comment,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }

    private static Material MapFromUpdateRequest(UpdateMaterialRequest request, int id)
    {
        return new Material
        {
            Id = id,
            MaterialGroupId = request.MaterialGroupId,
            Name = request.Name,
            Description = request.Description,
            MaterialNumber = request.MaterialNumber,
            ManufacturerReference = request.ManufacturerReference,
            DensityKilogramPerCubicMeter = request.DensityKilogramPerCubicMeter,
            TensileStrengthUltimateGigaPascal = request.TensileStrengthUltimateGigaPascal,
            TensileStrengthYieldMegaPascal = request.TensileStrengthYieldMegaPascal,
            MachinabilityPercent = request.MachinabilityPercent,
            ShearModulusGigaPascal = request.ShearModulusGigaPascal,
            ThermalConductivityWattPerMeterKelvin = request.ThermalConductivityWattPerMeterKelvin,
            PricePerKilogram = request.PricePerKilogram,
            CurrencyId = request.CurrencyId,
            Url = request.Url,
            Comment = request.Comment,
            IsActive = request.IsActive,
            ModifiedDate = DateTime.UtcNow
        };
    }
}