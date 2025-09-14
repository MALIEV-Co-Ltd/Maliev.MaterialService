using Asp.Versioning;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Maliev.MaterialService.Api.Controllers;

[ApiController]
[Route("materials/v{version:apiVersion}/processes")]
[ApiVersion("1.0")]
[EnableRateLimiting("MaterialsPolicy")]
[Authorize]
public class ManufacturingProcessesController : ControllerBase
{
    private readonly IManufacturingProcessService _processService;
    private readonly ILogger<ManufacturingProcessesController> _logger;

    public ManufacturingProcessesController(
        IManufacturingProcessService processService,
        ILogger<ManufacturingProcessesController> logger)
    {
        _processService = processService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ManufacturingProcessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<ManufacturingProcessDto>>> GetAll()
    {
        _logger.LogDebug("Getting all manufacturing processes");

        var processes = await _processService.GetAllProcessesAsync();
        var processDtos = processes.Select(p => new ManufacturingProcessDto
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Name = p.Name,
            Description = p.Description,
            SortOrder = p.SortOrder,
            Category = p.Category != null ? new ManufacturingProcessCategoryDto
            {
                Id = p.Category.Id,
                Name = p.Category.Name,
                Description = p.Category.Description,
                SortOrder = p.Category.SortOrder
            } : null
        });

        _logger.LogDebug("Retrieved {Count} manufacturing processes", processDtos.Count());
        return Ok(processDtos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ManufacturingProcessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ManufacturingProcessDto>> GetById(int id)
    {
        _logger.LogDebug("Getting manufacturing process by ID: {Id}", id);

        var process = await _processService.GetProcessByIdAsync(id);

        if (process == null)
        {
            _logger.LogWarning("Manufacturing process not found with ID: {Id}", id);
            return NotFound($"Manufacturing process with ID {id} not found");
        }

        var processDto = new ManufacturingProcessDto
        {
            Id = process.Id,
            CategoryId = process.CategoryId,
            Name = process.Name,
            Description = process.Description,
            SortOrder = process.SortOrder,
            Category = process.Category != null ? new ManufacturingProcessCategoryDto
            {
                Id = process.Category.Id,
                Name = process.Category.Name,
                Description = process.Category.Description,
                SortOrder = process.Category.SortOrder
            } : null
        };

        return Ok(processDto);
    }

    [HttpGet("category/{categoryId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ManufacturingProcessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IEnumerable<ManufacturingProcessDto>>> GetByCategory(int categoryId)
    {
        _logger.LogDebug("Getting manufacturing processes by category ID: {CategoryId}", categoryId);

        var processes = await _processService.GetProcessesByCategoryIdAsync(categoryId);
        var processDtos = processes.Select(p => new ManufacturingProcessDto
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Name = p.Name,
            Description = p.Description,
            SortOrder = p.SortOrder,
            Category = p.Category != null ? new ManufacturingProcessCategoryDto
            {
                Id = p.Category.Id,
                Name = p.Category.Name,
                Description = p.Category.Description,
                SortOrder = p.Category.SortOrder
            } : null
        });

        _logger.LogDebug("Retrieved {Count} manufacturing processes for category {CategoryId}", processDtos.Count(), categoryId);
        return Ok(processDtos);
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