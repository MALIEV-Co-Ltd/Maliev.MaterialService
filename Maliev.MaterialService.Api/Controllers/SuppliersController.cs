using Asp.Versioning;
using Maliev.MaterialService.Api.Services.Materials;
using Microsoft.AspNetCore.Mvc;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Controller for supplier-related queries in the Material Service
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("material/v{version:apiVersion}/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<SuppliersController> _logger;

    /// <summary>
    /// Initializes a new instance of the SuppliersController
    /// </summary>
    /// <param name="materialService">Material service for business logic</param>
    /// <param name="logger">Logger instance</param>
    public SuppliersController(IMaterialService materialService, ILogger<SuppliersController> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    /// <summary>
    /// Check how many materials reference a specific supplier
    /// </summary>
    /// <remarks>
    /// This endpoint is used by other services (e.g., Supplier Service) to verify
    /// if a supplier can be safely deleted by checking for material references.
    /// </remarks>
    /// <param name="supplierId">The supplier ID to check</param>
    /// <returns>Reference count for the supplier</returns>
    [HttpGet("{supplierId:guid}/references")]
    [ProducesResponseType(typeof(SupplierReferenceResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SupplierReferenceResponse>> GetSupplierReferences(Guid supplierId)
    {
        _logger.LogInformation("Checking material references for supplier {SupplierId}", supplierId);

        var referenceCount = await _materialService.GetSupplierReferenceCountAsync(supplierId);

        return Ok(new SupplierReferenceResponse(referenceCount));
    }
}

/// <summary>
/// Response containing the count of references to a supplier
/// </summary>
/// <param name="ReferenceCount">Number of materials referencing the supplier</param>
public record SupplierReferenceResponse(int ReferenceCount);
