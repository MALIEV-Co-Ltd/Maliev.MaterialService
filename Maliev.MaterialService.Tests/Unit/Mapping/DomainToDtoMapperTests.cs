using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Domain.Entities;
using Maliev.MaterialService.Infrastructure.Mapping;
using Xunit;

namespace Maliev.MaterialService.Tests.Unit.Mapping;

public class DomainToDtoMapperTests
{
    [Fact]
    public void ToMaterialResponse_MapsAllFields()
    {
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = "Test Material",
            Code = "TM-001",
            Description = "Test Description",
            PricePerUnit = 10.5m,
            StockLevel = 100,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "creator",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "updater",
            Version = new byte[] { 1, 2, 3 }
        };

        var response = material.ToMaterialResponse();

        Assert.Equal(material.Id, response.Id);
        Assert.Equal(material.Name, response.Name);
        Assert.Equal(material.Code, response.Code);
        Assert.Equal(material.Description, response.Description);
        Assert.Equal(material.PricePerUnit, response.PricePerUnit);
        Assert.Equal(material.StockLevel, response.StockLevel);
        Assert.Equal(material.Active, response.Active);
        Assert.Equal(material.CreatedAt, response.CreatedAt);
        Assert.Equal(material.CreatedBy, response.CreatedBy);
        Assert.Equal(material.UpdatedAt, response.UpdatedAt);
        Assert.Equal(material.UpdatedBy, response.UpdatedBy);
        Assert.Equal(material.Version, response.Version);
    }

    [Fact]
    public void ToMaterial_MapsRequestToEntity()
    {
        var request = new CreateMaterialRequest
        {
            Name = "New Material",
            Code = "NM-001",
            Description = "New Description",
            PricePerUnit = 20.0m,
            StockLevel = 50
        };

        var material = request.ToMaterial();

        Assert.Equal(request.Name, material.Name);
        Assert.Equal(request.Code, material.Code);
        Assert.Equal(request.Description, material.Description);
        Assert.Equal(request.PricePerUnit, material.PricePerUnit);
        Assert.Equal(request.StockLevel, material.StockLevel);
    }

    [Fact]
    public void UpdateMaterial_UpdatesAllFields()
    {
        var material = new Material { Name = "Old", Code = "OLD" };
        var request = new UpdateMaterialRequest
        {
            Name = "New",
            Code = "NEW",
            Description = "Desc",
            PricePerUnit = 15m,
            StockLevel = 10
        };

        material.UpdateMaterial(request);

        Assert.Equal(request.Name, material.Name);
        Assert.Equal(request.Code, material.Code);
        Assert.Equal(request.Description, material.Description);
        Assert.Equal(request.PricePerUnit, material.PricePerUnit);
        Assert.Equal(request.StockLevel, material.StockLevel);
    }

    [Fact]
    public void ToManufacturingProcessResponse_MapsFieldsCorrectly()
    {
        var process = new ManufacturingProcess
        {
            Id = Guid.NewGuid(),
            Name = "CNC Machining"
        };

        var response = process.ToManufacturingProcessResponse();

        Assert.Equal(process.Id, response.Id);
        Assert.Equal(process.Name, response.Name);
    }

    [Fact]
    public void ToColorResponse_MapsFieldsCorrectly()
    {
        var color = new Color
        {
            Id = Guid.NewGuid(),
            Name = "Blue",
            HexCode = "#0000FF"
        };

        var response = color.ToColorResponse();

        Assert.Equal(color.Id, response.Id);
        Assert.Equal(color.Name, response.Name);
        Assert.Equal(color.HexCode, response.HexCode);
    }

    [Fact]
    public void ToPostProcessingMethodResponse_MapsFieldsCorrectly()
    {
        var method = new PostProcessingMethod
        {
            Id = Guid.NewGuid(),
            Name = "Sanding"
        };

        var response = method.ToPostProcessingMethodResponse();

        Assert.Equal(method.Id, response.Id);
        Assert.Equal(method.Name, response.Name);
    }

    [Fact]
    public void ToMaterialMechanicalPropertyResponse_MapsFieldsCorrectly()
    {
        var property = new MaterialMechanicalProperty
        {
            MaterialId = Guid.NewGuid(),
            MechanicalPropertyId = Guid.NewGuid(),
            Value = 100.5m,
            MechanicalProperty = new MechanicalProperty
            {
                Name = "Tensile Strength",
                Unit = "MPa"
            }
        };

        var response = property.ToMaterialMechanicalPropertyResponse();

        Assert.Equal(property.MechanicalPropertyId, response.MechanicalPropertyId);
        Assert.Equal("Tensile Strength", response.MechanicalPropertyName);
        Assert.Equal(100.5m, response.Value);
        Assert.Equal("MPa", response.Unit);
    }

    [Fact]
    public void ToMaterialMechanicalPropertyResponse_WithNullMechanicalProperty_ReturnsEmptyStrings()
    {
        var property = new MaterialMechanicalProperty
        {
            MaterialId = Guid.NewGuid(),
            MechanicalPropertyId = Guid.NewGuid(),
            Value = 100.5m,
            MechanicalProperty = null!
        };

        var response = property.ToMaterialMechanicalPropertyResponse();

        Assert.Equal(property.MechanicalPropertyId, response.MechanicalPropertyId);
        Assert.Equal(string.Empty, response.MechanicalPropertyName);
        Assert.Equal(100.5m, response.Value);
        Assert.Equal(string.Empty, response.Unit);
    }

    [Fact]
    public void ToMaterialResponse_WithNullCollections_ReturnsEmptyLists()
    {
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = "Test Material",
            Code = "TM-001",
            PricePerUnit = 10.5m,
            StockLevel = 100,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            ManufacturingProcesses = null!,
            AvailableColors = null!,
            PostProcessingMethods = null!,
            MechanicalProperties = null!
        };

        var response = material.ToMaterialResponse();

        Assert.NotNull(response.ManufacturingProcesses);
        Assert.Empty(response.ManufacturingProcesses);
        Assert.NotNull(response.AvailableColors);
        Assert.Empty(response.AvailableColors);
        Assert.NotNull(response.PostProcessingMethods);
        Assert.Empty(response.PostProcessingMethods);
        Assert.NotNull(response.MechanicalProperties);
        Assert.Empty(response.MechanicalProperties);
    }

    [Fact]
    public void ToMaterialResponse_WithSupplier_MapsSupplierName()
    {
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = "Test Material",
            Code = "TM-001",
            PricePerUnit = 10.5m,
            StockLevel = 100,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            Supplier = new Supplier { Name = "Test Supplier" }
        };

        var response = material.ToMaterialResponse();

        Assert.Equal("Test Supplier", response.SupplierName);
    }
}
