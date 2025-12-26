using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.Cache;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Controller for retrieving reference data (colors, processes, methods)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("material/v{version:apiVersion}/reference")]
[RequirePermission("material.categories.read")]
public class ReferenceDataController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private const string ColorsKey = "ref:colors";
    private const string ProcessesKey = "ref:processes";
    private const string MethodsKey = "ref:methods";

    /// <summary>
    /// Initializes a new instance of ReferenceDataController
    /// </summary>
    /// <param name="cacheService">Cache service for retrieving reference data</param>
    public ReferenceDataController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <summary>
    /// Get all available colors
    /// </summary>
    /// <returns>List of colors</returns>
    [HttpGet("colors")]
    public async Task<ActionResult<IEnumerable<ColorResponse>>> GetColors()
    {
        var colors = await _cacheService.GetAsync<List<ColorResponse>>(ColorsKey);
        return Ok(colors ?? new List<ColorResponse>());
    }

    /// <summary>
    /// Get all manufacturing processes
    /// </summary>
    /// <returns>List of manufacturing processes</returns>
    [HttpGet("processes")]
    public async Task<ActionResult<IEnumerable<ManufacturingProcessResponse>>> GetManufacturingProcesses()
    {
        var processes = await _cacheService.GetAsync<List<ManufacturingProcessResponse>>(ProcessesKey);
        return Ok(processes ?? new List<ManufacturingProcessResponse>());
    }

    /// <summary>
    /// Get all post-processing methods
    /// </summary>
    /// <returns>List of post-processing methods</returns>
    [HttpGet("methods")]
    public async Task<ActionResult<IEnumerable<PostProcessingMethodResponse>>> GetPostProcessingMethods()
    {
        var methods = await _cacheService.GetAsync<List<PostProcessingMethodResponse>>(MethodsKey);
        return Ok(methods ?? new List<PostProcessingMethodResponse>());
    }
}
