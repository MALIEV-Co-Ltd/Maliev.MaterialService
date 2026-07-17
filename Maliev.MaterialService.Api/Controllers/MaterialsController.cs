using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.MaterialService.Application.Authorization;
using Maliev.MaterialService.Application.DTOs;
using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Controller for managing materials
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("material/v{version:apiVersion}/materials")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<MaterialsController> _logger;

    /// <summary>
    /// Initializes a new instance of MaterialsController
    /// </summary>
    /// <param name="materialService">Material service</param>
    /// <param name="logger">Logger instance</param>
    public MaterialsController(IMaterialService materialService, ILogger<MaterialsController> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    /// <summary>
    /// Get materials with optional filtering, sorting, and pagination.
    /// </summary>
    /// <remarks>
    /// Public endpoint to browse the material catalog.
    /// Supports extensive filtering by price, supplier, color, and technical properties.
    /// </remarks>
    /// <response code="200">Returns the requested page of materials.</response>
    [HttpGet]
    [RequirePermission(MaterialPermissions.MaterialsRead)]
    [ProducesResponseType(typeof(PagedResult<MaterialResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MaterialResponse>>> GetMaterials(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] Guid? supplierId = null,
        [FromQuery] string? manufacturingProcess = null,
        [FromQuery] string? color = null,
        [FromQuery] decimal? minTensileStrength = null,
        [FromQuery] decimal? maxTensileStrength = null)
    {
        _logger.LogInformation("Getting materials with pagination: page={Page}, pageSize={PageSize}", page, pageSize);

        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Limit max page size

        var result = await _materialService.GetMaterialsAsync(
            page, pageSize, search, sortBy, sortDesc, minPrice, maxPrice, supplierId, manufacturingProcess, color, minTensileStrength, maxTensileStrength);

        return Ok(result);
    }

    /// <summary>
    /// Get a material by ID.
    /// </summary>
    /// <remarks>
    /// Fetches full technical details and supplier information for a specific material.
    /// </remarks>
    /// <response code="403">If the user lacks `material.materials.read` permission.</response>
    /// <response code="404">If the material ID is not found.</response>
    [HttpGet("{id}")]
    [RequirePermission(MaterialPermissions.MaterialsRead)]
    [ProducesResponseType(typeof(MaterialResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaterialResponse>> GetMaterialById(Guid id)
    {
        _logger.LogInformation("Getting material with ID: {MaterialId}", id);
        var material = await _materialService.GetMaterialByIdAsync(id);

        if (material == null)
        {
            return NotFound(new { message = $"Material with ID {id} not found." });
        }

        return Ok(material);
    }

    /// <summary>
    /// Create a new material.
    /// </summary>
    /// <remarks>
    /// Adds a new material to the catalog.
    /// </remarks>
    /// <response code="400">If the request data is invalid.</response>
    /// <response code="403">If the user lacks `material.materials.create` permission.</response>
    [HttpPost]
    [RequirePermission(MaterialPermissions.MaterialsCreate)]
    [ProducesResponseType(typeof(MaterialResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MaterialResponse>> CreateMaterial(
        [FromBody] CreateMaterialRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new material with code: {Code}", request.Code);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var material = await _materialService.CreateMaterialAsync(request, userId, cancellationToken);

            return CreatedAtAction(
                nameof(GetMaterialById),
                new { id = material.Id },
                material);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create material");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing material.
    /// </summary>
    /// <remarks>
    /// Updates technical properties or pricing. Implements optimistic concurrency.
    /// </remarks>
    /// <response code="200">Updated successfully.</response>
    /// <response code="403">If the user lacks `material.materials.update` permission.</response>
    /// <response code="409">If the material was modified by another user since it was fetched.</response>
    [HttpPut("{id}")]
    [RequirePermission(MaterialPermissions.MaterialsUpdate)]
    [ProducesResponseType(typeof(MaterialResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MaterialResponse>> UpdateMaterial(
        Guid id,
        [FromBody] UpdateMaterialRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating material with ID: {MaterialId}", id);

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            var material = await _materialService.UpdateMaterialAsync(id, request, userId, cancellationToken);

            if (material == null)
            {
                return NotFound(new { message = $"Material with ID {id} not found." });
            }

            return Ok(material);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict when updating material");
            return Conflict(new { message = "The material has been modified by another user. Please refresh and try again." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update material");
            if (ex.Message.Contains("modified by another user"))
            {
                return Conflict(new { message = ex.Message });
            }
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a material.
    /// </summary>
    /// <remarks>
    /// **CRITICAL:** Soft-deletes the material. Historical order references will still function.
    /// </remarks>
    /// <response code="204">Successfully deleted.</response>
    /// <response code="403">If the user lacks `material.materials.delete` permission.</response>
    [HttpDelete("{id}")]
    [RequirePermission(MaterialPermissions.MaterialsDelete, IsCritical = true)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteMaterial(Guid id)
    {
        _logger.LogInformation("Deleting material with ID: {MaterialId}", id);

        var result = await _materialService.DeleteMaterialAsync(id);

        if (!result)
        {
            return NotFound(new { message = $"Material with ID {id} not found." });
        }

        return NoContent();
    }
}
