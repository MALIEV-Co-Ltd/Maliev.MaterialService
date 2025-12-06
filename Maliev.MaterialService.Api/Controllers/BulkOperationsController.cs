using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp.Versioning;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.Bulk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Controller for bulk material operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("materials/v{version:apiVersion}/bulk")]
public class BulkOperationsController : ControllerBase
{
    private readonly IBulkMaterialService _bulkMaterialService;
    private readonly ILogger<BulkOperationsController> _logger;

    /// <summary>
    /// Initializes a new instance of BulkOperationsController
    /// </summary>
    /// <param name="bulkMaterialService">Bulk material service</param>
    /// <param name="logger">Logger instance</param>
    public BulkOperationsController(IBulkMaterialService bulkMaterialService, ILogger<BulkOperationsController> logger)
    {
        _bulkMaterialService = bulkMaterialService;
        _logger = logger;
    }

    /// <summary>
    /// Bulk import materials from JSON
    /// </summary>
    [HttpPost("import")]
    [Authorize(Policy = "EmployeeOrHigher")]
    [ProducesResponseType(typeof(BulkImportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BulkImportResponse>> BulkImportMaterials([FromBody] BulkImportRequest request)
    {
        _logger.LogInformation("Bulk import request received for {Count} materials", request.Materials.Count);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var result = await _bulkMaterialService.BulkImportMaterialsAsync(request, userId);

        if (result.FailureCount == result.TotalCount)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Bulk export all materials as JSON
    /// </summary>
    [HttpGet("export")]
    [Authorize(Policy = "EmployeeOrHigher")]
    [ProducesResponseType(typeof(IEnumerable<MaterialResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MaterialResponse>>> BulkExportMaterials()
    {
        _logger.LogInformation("Bulk export request received");
        var materials = await _bulkMaterialService.BulkExportMaterialsAsync();
        return Ok(materials);
    }
}
