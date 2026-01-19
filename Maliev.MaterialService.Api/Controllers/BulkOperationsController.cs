using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.Auth;
using Maliev.MaterialService.Api.Services.Bulk;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Controller for bulk material operations
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("material/v{version:apiVersion}/bulk")]
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
    /// Bulk import materials from JSON file.
    /// </summary>
    [HttpPost("import")]
    [RequirePermission(MaterialPermissions.MaterialsCreate)]
    [Consumes("multipart/form-data")] // Specify content type for file upload
    [ProducesResponseType(typeof(BulkImportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)] // Add unsupported media type
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BulkImportResponse>> BulkImportMaterials(
        IFormFile file,
        [FromQuery] bool dryRun = false,
        [FromQuery] bool validateOnly = false)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        if (file.ContentType != "application/json") // Assuming JSON format for now
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType, new { message = "Unsupported media type. Only application/json is accepted." });
        }

        List<CreateMaterialRequest>? materialsToImport;
        try
        {
            using var stream = file.OpenReadStream();
            materialsToImport = await JsonSerializer.DeserializeAsync<List<CreateMaterialRequest>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (materialsToImport == null || materialsToImport.Count == 0)
            {
                return BadRequest(new { message = "No materials found in the uploaded JSON file." });
            }
        }
        catch (JsonException ex)
        {
            return BadRequest(new { message = $"Invalid JSON format: {ex.Message}" });
        }

        _logger.LogInformation("Bulk import request received for {Count} materials. DryRun: {DryRun}, ValidateOnly: {ValidateOnly}",
            materialsToImport.Count, dryRun, validateOnly);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        var request = new BulkImportRequest
        {
            Materials = materialsToImport,
            DryRun = dryRun,
            ValidateOnly = validateOnly
        };
        var result = await _bulkMaterialService.BulkImportMaterialsAsync(request, userId);

        if (result.FailureCount > 0)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Bulk export all materials as JSON or CSV
    /// </summary>
    [HttpGet("export")]
    [RequirePermission(MaterialPermissions.MaterialsExport)]
    [ProducesResponseType(typeof(IEnumerable<MaterialResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces("application/json", "text/csv")] // Allow multiple content types
    public async Task<IActionResult> BulkExportMaterials([FromQuery] string format = "json")
    {
        _logger.LogInformation("Bulk export request received. Format: {Format}", format);
        var materials = await _bulkMaterialService.BulkExportMaterialsAsync();

        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

            // Write Header
            await writer.WriteLineAsync("Code,Name,PricePerUnit,StockLevel");

            // Write Records
            foreach (var m in materials)
            {
                var line = $"{EscapeCsv(m.Code)},{EscapeCsv(m.Name)},{m.PricePerUnit.ToString(CultureInfo.InvariantCulture)},{m.StockLevel}";
                await writer.WriteLineAsync(line);
            }

            await writer.FlushAsync();
            stream.Position = 0;
            return File(stream, "text/csv", "materials.csv");
        }

        return Ok(materials);
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
