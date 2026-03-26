using Asp.Versioning;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.MaterialService.Application.Authorization;
using Maliev.MaterialService.Application.DTOs.Catalog;
using Maliev.MaterialService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maliev.MaterialService.Api.Controllers;

/// <summary>
/// Read-only catalog API for manufacturing processes, materials, finishes, tolerances, and config options.
/// </summary>
[ApiController]
[ApiVersion("1")]
[Route("material/v{version:apiVersion}/manufacturing")]
[RequirePermission(MaterialPermissions.CategoriesRead)]
public class ManufacturingCatalogController(MaterialDbContext db) : ControllerBase
{
    /// <summary>Returns all active manufacturing processes.</summary>
    [HttpGet("processes")]
    public async Task<ActionResult<IEnumerable<ProcessCatalogResponse>>> GetProcesses()
    {
        var processes = await db.ManufacturingProcesses
            .Where(p => p.Active)
            .OrderBy(p => p.SortOrder)
            .Select(p => new ProcessCatalogResponse
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                SortOrder = p.SortOrder,
            })
            .ToListAsync();

        return Ok(processes);
    }

    /// <summary>Returns all active materials for the given process code.</summary>
    [HttpGet("processes/{processCode}/materials")]
    public async Task<ActionResult<IEnumerable<MaterialCatalogResponse>>> GetMaterialsByProcess(string processCode)
    {
        var process = await db.ManufacturingProcesses
            .Where(p => p.Active && p.Code == processCode.ToUpperInvariant())
            .Include(p => p.Materials)
            .FirstOrDefaultAsync();

        if (process is null) return NotFound();

        var materials = process.Materials
            .Where(m => m.Active)
            .OrderBy(m => m.SortOrder)
            .Select(m => new MaterialCatalogResponse
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                Category = m.Category,
                DensityGCm3 = m.DensityGCm3,
                Description = m.Description,
                SortOrder = m.SortOrder,
            })
            .ToList();

        return Ok(materials);
    }

    /// <summary>Returns all active surface finishes for the given process code.</summary>
    [HttpGet("processes/{processCode}/finishes")]
    public async Task<ActionResult<IEnumerable<SurfaceFinishCatalogResponse>>> GetFinishesByProcess(string processCode)
    {
        var processId = await db.ManufacturingProcesses
            .Where(p => p.Active && p.Code == processCode.ToUpperInvariant())
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync();

        if (processId is null) return NotFound();

        var finishes = await db.SurfaceFinishes
            .Where(sf => sf.Active && sf.AvailableForProcesses.Any(p => p.Id == processId))
            .OrderBy(sf => sf.SortOrder)
            .Select(sf => new SurfaceFinishCatalogResponse
            {
                Id = sf.Id,
                Name = sf.Name,
                Code = sf.Code,
                RaValueUm = sf.RaValueUm,
                AdditionalCostPercent = sf.AdditionalCostPercent,
                Description = sf.Description,
                SortOrder = sf.SortOrder,
            })
            .ToListAsync();

        return Ok(finishes);
    }

    /// <summary>Returns all active tolerance classes for the given process code.</summary>
    [HttpGet("processes/{processCode}/tolerances")]
    public async Task<ActionResult<IEnumerable<ToleranceClassCatalogResponse>>> GetTolerancesByProcess(string processCode)
    {
        var processId = await db.ManufacturingProcesses
            .Where(p => p.Active && p.Code == processCode.ToUpperInvariant())
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync();

        if (processId is null) return NotFound();

        var tolerances = await db.ToleranceClasses
            .Where(tc => tc.Active && tc.AvailableForProcesses.Any(p => p.Id == processId))
            .OrderBy(tc => tc.SortOrder)
            .Select(tc => new ToleranceClassCatalogResponse
            {
                Id = tc.Id,
                Name = tc.Name,
                Code = tc.Code,
                IsoStandard = tc.IsoStandard,
                Grade = tc.Grade,
                ToleranceRange = tc.ToleranceRange,
                AdditionalCostPercent = tc.AdditionalCostPercent,
                SortOrder = tc.SortOrder,
            })
            .ToListAsync();

        return Ok(tolerances);
    }

    /// <summary>Returns all active configuration options for the given process code.</summary>
    [HttpGet("processes/{processCode}/config-options")]
    public async Task<ActionResult<IEnumerable<ProcessConfigOptionCatalogResponse>>> GetConfigOptionsByProcess(string processCode)
    {
        var processId = await db.ManufacturingProcesses
            .Where(p => p.Active && p.Code == processCode.ToUpperInvariant())
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync();

        if (processId is null) return NotFound();

        var options = await db.ProcessConfigOptions
            .Where(o => o.Active && o.ManufacturingProcessId == processId)
            .OrderBy(o => o.SortOrder)
            .Select(o => new ProcessConfigOptionCatalogResponse
            {
                Id = o.Id,
                ConfigKey = o.ConfigKey,
                Label = o.Label,
                ConfigType = o.ConfigType,
                DefaultValue = o.DefaultValue,
                OptionsJson = o.OptionsJson,
                Unit = o.Unit,
                HelpText = o.HelpText,
                IsRequired = o.IsRequired,
                SortOrder = o.SortOrder,
            })
            .ToListAsync();

        return Ok(options);
    }

    /// <summary>Returns surface finishes compatible with a specific material.</summary>
    [HttpGet("materials/{materialId:guid}/finishes")]
    public async Task<ActionResult<IEnumerable<SurfaceFinishCatalogResponse>>> GetFinishesByMaterial(Guid materialId)
    {
        var finishes = await db.SurfaceFinishes
            .Where(sf => sf.Active && sf.CompatibleMaterials.Any(m => m.Id == materialId))
            .OrderBy(sf => sf.SortOrder)
            .Select(sf => new SurfaceFinishCatalogResponse
            {
                Id = sf.Id,
                Name = sf.Name,
                Code = sf.Code,
                RaValueUm = sf.RaValueUm,
                AdditionalCostPercent = sf.AdditionalCostPercent,
                Description = sf.Description,
                SortOrder = sf.SortOrder,
            })
            .ToListAsync();

        return Ok(finishes);
    }
}
