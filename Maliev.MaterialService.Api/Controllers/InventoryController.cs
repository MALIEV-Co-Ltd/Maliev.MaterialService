using Asp.Versioning;
using Maliev.MaterialService.Api.Services.Auth;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Controller for inventory-related operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Microsoft.AspNetCore.Authorization.Authorize]
[Route("material/v{version:apiVersion}/inventory")]
public class InventoryController : ControllerBase
{
    private readonly AuthMetrics _metrics;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(AuthMetrics metrics, ILogger<InventoryController> logger)
    {
        _metrics = metrics;
        _logger = logger;
    }

    /// <summary>
    /// Get stock level for a material
    /// </summary>
    [HttpGet("{materialId:guid}")]
    [RequirePermission("material.inventory.view")]
    public IActionResult GetStockLevel(Guid materialId)
    {
        _metrics.RecordSuccess("material.inventory.view");
        _logger.LogInformation("Retrieving inventory level for material {MaterialId}", materialId);
        return Ok(new { materialId, stockLevel = 100 });
    }

    /// <summary>
    /// Perform an inventory count for a material
    /// </summary>
    [HttpPost("count")]
    [RequirePermission("material.inventory.count")]
    public IActionResult RecordCount([FromBody] InventoryCountRequest request)
    {
        _metrics.RecordSuccess("material.inventory.count");
        _logger.LogInformation("Recording inventory count for material {MaterialId}", request.MaterialId);
        return Ok(new { message = "Count recorded successfully" });
    }

    /// <summary>
    /// Adjust inventory level for a material
    /// </summary>
    [HttpPost("adjust")]
    [RequirePermission("material.inventory.adjust", IsCritical = true)]
    public IActionResult AdjustStock([FromBody] InventoryAdjustmentRequest request)
    {
        _metrics.RecordSuccess("material.inventory.adjust");
        _logger.LogInformation("Adjusting inventory for material {MaterialId} by {Amount}", request.MaterialId, request.Adjustment);
        return Ok(new { message = "Adjustment successful" });
    }
}

public record InventoryCountRequest(Guid MaterialId, int Count);
public record InventoryAdjustmentRequest(Guid MaterialId, int Adjustment, string Reason);
