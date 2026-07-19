using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Domain.Entities;

namespace Maliev.MaterialService.Infrastructure.Mapping;

/// <summary>
/// Pure .NET mapper for material-related domain models and DTOs.
/// </summary>
public static class DomainToDtoMapper
{
    /// <summary>Maps a Material entity to MaterialResponse DTO.</summary>
    /// <param name="material">The material entity to map.</param>
    /// <returns>Mapped <see cref="MaterialResponse"/>.</returns>
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
            Active = material.Active,
            CreatedAt = material.CreatedAt,
            CreatedBy = material.CreatedBy,
            UpdatedAt = material.UpdatedAt,
            UpdatedBy = material.UpdatedBy,
            ManufacturingProcesses = material.ManufacturingProcesses?.Select(ToManufacturingProcessResponse).ToList() ?? new List<ManufacturingProcessResponse>(),
            AvailableColors = material.AvailableColors?.Select(ToColorResponse).ToList() ?? new List<ColorResponse>(),
            PostProcessingMethods = material.PostProcessingMethods?.Select(ToPostProcessingMethodResponse).ToList() ?? new List<PostProcessingMethodResponse>(),
            MechanicalProperties = material.MechanicalProperties?.Select(ToMaterialMechanicalPropertyResponse).ToList() ?? new List<MaterialMechanicalPropertyResponse>()
        };
    }

    /// <summary>Maps CreateMaterialRequest to Material entity.</summary>
    /// <param name="request">The create request to map.</param>
    /// <returns>Mapped <see cref="Material"/>.</returns>
    public static Material ToMaterial(this CreateMaterialRequest request)
    {
        return new Material
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            SupplierId = request.SupplierId,
            PricePerUnit = request.PricePerUnit,
            StockLevel = request.StockLevel
        };
    }

    /// <summary>Updates a Material entity from UpdateMaterialRequest.</summary>
    /// <param name="material">The material entity to update.</param>
    /// <param name="request">The update request.</param>
    public static void UpdateMaterial(this Material material, UpdateMaterialRequest request)
    {
        material.Name = request.Name;
        material.Code = request.Code;
        material.Description = request.Description;
        material.SupplierId = request.SupplierId;
        material.PricePerUnit = request.PricePerUnit;
        material.StockLevel = request.StockLevel;
    }

    /// <summary>Maps ManufacturingProcess to ManufacturingProcessResponse.</summary>
    /// <param name="process">The process entity to map.</param>
    /// <returns>Mapped <see cref="ManufacturingProcessResponse"/>.</returns>
    public static ManufacturingProcessResponse ToManufacturingProcessResponse(this ManufacturingProcess process)
    {
        return new ManufacturingProcessResponse
        {
            Id = process.Id,
            Name = process.Name
        };
    }

    /// <summary>Maps Color to ColorResponse.</summary>
    /// <param name="color">The color entity to map.</param>
    /// <returns>Mapped <see cref="ColorResponse"/>.</returns>
    public static ColorResponse ToColorResponse(this Color color)
    {
        return new ColorResponse
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        };
    }

    /// <summary>Maps PostProcessingMethod to PostProcessingMethodResponse.</summary>
    /// <param name="method">The post-processing method entity to map.</param>
    /// <returns>Mapped <see cref="PostProcessingMethodResponse"/>.</returns>
    public static PostProcessingMethodResponse ToPostProcessingMethodResponse(this PostProcessingMethod method)
    {
        return new PostProcessingMethodResponse
        {
            Id = method.Id,
            Name = method.Name
        };
    }

    /// <summary>Maps MaterialMechanicalProperty to MaterialMechanicalPropertyResponse.</summary>
    /// <param name="property">The mechanical property join entity to map.</param>
    /// <returns>Mapped <see cref="MaterialMechanicalPropertyResponse"/>.</returns>
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
