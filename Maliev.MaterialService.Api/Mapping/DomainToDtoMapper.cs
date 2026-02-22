using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Mapping;

/// <summary>
/// Pure .NET mapper for material-related domain models and DTOs
/// </summary>
public static class DomainToDtoMapper
{
    /// <summary>Maps a Material entity to MaterialResponse DTO</summary>
    public static MaterialResponse ToMaterialResponse(this Material material)
    {
        return new MaterialResponse
        {
            Id = material.Id,
            Name = material.Name,
            Code = material.Code,
            Description = material.Description,
            SupplierId = material.SupplierId,
            SupplierName = material.Supplier?.Name,
            PricePerUnit = material.PricePerUnit,
            StockLevel = material.StockLevel,
            Density = material.Density,
            CostPerKg = material.CostPerKg,
            ProcessParameters = material.ProcessParameters,
            Active = material.Active,
            CreatedAt = material.CreatedAt,
            CreatedBy = material.CreatedBy,
            UpdatedAt = material.UpdatedAt,
            UpdatedBy = material.UpdatedBy,
            Version = material.Version,
            ManufacturingProcesses = material.ManufacturingProcesses?.Select(ToManufacturingProcessResponse).ToList() ?? new List<ManufacturingProcessResponse>(),
            AvailableColors = material.AvailableColors?.Select(ToColorResponse).ToList() ?? new List<ColorResponse>(),
            PostProcessingMethods = material.PostProcessingMethods?.Select(ToPostProcessingMethodResponse).ToList() ?? new List<PostProcessingMethodResponse>(),
            MechanicalProperties = material.MechanicalProperties?.Select(ToMaterialMechanicalPropertyResponse).ToList() ?? new List<MaterialMechanicalPropertyResponse>()
        };
    }

    /// <summary>Maps CreateMaterialRequest to Material entity</summary>
    public static Material ToMaterial(this CreateMaterialRequest request)
    {
        return new Material
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            SupplierId = request.SupplierId,
            PricePerUnit = request.PricePerUnit,
            StockLevel = request.StockLevel,
            Density = request.Density,
            CostPerKg = request.CostPerKg,
            ProcessParameters = request.ProcessParameters
        };
    }

    /// <summary>Updates a Material entity from UpdateMaterialRequest</summary>
    public static void UpdateMaterial(this Material material, UpdateMaterialRequest request)
    {
        material.Name = request.Name;
        material.Code = request.Code;
        material.Description = request.Description;
        material.SupplierId = request.SupplierId;
        material.PricePerUnit = request.PricePerUnit;
        material.StockLevel = request.StockLevel;
        material.Density = request.Density;
        material.CostPerKg = request.CostPerKg;
        material.ProcessParameters = request.ProcessParameters;
    }

    /// <summary>Maps ManufacturingProcess to ManufacturingProcessResponse</summary>
    public static ManufacturingProcessResponse ToManufacturingProcessResponse(this ManufacturingProcess process)
    {
        return new ManufacturingProcessResponse
        {
            Id = process.Id,
            Name = process.Name
        };
    }

    /// <summary>Maps Color to ColorResponse</summary>
    public static ColorResponse ToColorResponse(this Color color)
    {
        return new ColorResponse
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        };
    }

    /// <summary>Maps PostProcessingMethod to PostProcessingMethodResponse</summary>
    public static PostProcessingMethodResponse ToPostProcessingMethodResponse(this PostProcessingMethod method)
    {
        return new PostProcessingMethodResponse
        {
            Id = method.Id,
            Name = method.Name
        };
    }

    /// <summary>Maps MaterialMechanicalProperty to MaterialMechanicalPropertyResponse</summary>
    public static MaterialMechanicalPropertyResponse ToMaterialMechanicalPropertyResponse(this MaterialMechanicalProperty property)
    {
        return new MaterialMechanicalPropertyResponse
        {
            MechanicalPropertyId = property.MechanicalPropertyId,
            MechanicalPropertyName = property.MechanicalProperty?.Name ?? string.Empty,
            Value = property.Value,
            Unit = property.MechanicalProperty?.Unit ?? string.Empty
        };
    }
}
