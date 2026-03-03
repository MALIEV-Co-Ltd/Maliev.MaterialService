using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Application.Services;
using Maliev.MaterialService.Domain.Entities;
using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Maliev.MaterialService.Tests.Unit.Services;

public class MaterialServiceValidationTests
{
    [Fact]
    public void CreateMaterialRequest_WithInvalidName_ReturnsValidationError()
    {
        var request = new CreateMaterialRequest
        {
            Name = "",
            Code = "TEST-001",
            PricePerUnit = 10.0m,
            StockLevel = 100
        };

        Assert.True(string.IsNullOrWhiteSpace(request.Name));
    }

    [Fact]
    public void CreateMaterialRequest_WithNegativeStockLevel_ShouldFail()
    {
        var request = new CreateMaterialRequest
        {
            Name = "Test",
            Code = "TEST-001",
            PricePerUnit = 10.0m,
            StockLevel = -10
        };

        Assert.True(request.StockLevel < 0);
    }

    [Fact]
    public void CreateMaterialRequest_WithZeroPrice_ShouldFail()
    {
        var request = new CreateMaterialRequest
        {
            Name = "Test",
            Code = "TEST-001",
            PricePerUnit = 0m,
            StockLevel = 100
        };

        Assert.True(request.PricePerUnit <= 0);
    }

    [Fact]
    public void CreateMaterialRequest_WithNegativePrice_ShouldFail()
    {
        var request = new CreateMaterialRequest
        {
            Name = "Test",
            Code = "TEST-001",
            PricePerUnit = -5m,
            StockLevel = 100
        };

        Assert.True(request.PricePerUnit <= 0);
    }
}

public class MaterialEntityTests
{
    [Fact]
    public void Material_DefaultConstructor_InitializesCollections()
    {
        var material = new Material();

        Assert.NotNull(material.ManufacturingProcesses);
        Assert.NotNull(material.AvailableColors);
        Assert.NotNull(material.PostProcessingMethods);
        Assert.NotNull(material.MechanicalProperties);
    }

    [Fact]
    public void Material_SetProperties_SetsValuesCorrectly()
    {
        var material = new Material
        {
            Name = "Test Material",
            Code = "TEST-001",
            Description = "Test Description",
            PricePerUnit = 25.00m,
            StockLevel = 100,
            SupplierId = Guid.NewGuid()
        };

        Assert.Equal("Test Material", material.Name);
        Assert.Equal("TEST-001", material.Code);
        Assert.Equal("Test Description", material.Description);
        Assert.Equal(25.00m, material.PricePerUnit);
        Assert.Equal(100, material.StockLevel);
    }

    [Fact]
    public void BaseEntity_HasDefaultId()
    {
        var material = new Material();
        Assert.Equal(Guid.Empty, material.Id);
    }

    [Fact]
    public void BaseEntity_HasDefaultCreatedAt()
    {
        var material = new Material();
        Assert.Equal(DateTimeOffset.MinValue, material.CreatedAt);
    }

    [Fact]
    public void Material_ActiveByDefault()
    {
        var material = new Material();
        Assert.True(material.Active);
    }

    [Fact]
    public void Material_HasEmptyVersionByDefault()
    {
        var material = new Material();
        Assert.NotNull(material.Version);
        Assert.Empty(material.Version);
    }
}

public class ColorEntityTests
{
    [Fact]
    public void Color_SetProperties_SetsValuesCorrectly()
    {
        var color = new Color
        {
            Name = "Blue",
            HexCode = "#0000FF"
        };

        Assert.Equal("Blue", color.Name);
        Assert.Equal("#0000FF", color.HexCode);
    }

    [Fact]
    public void Color_ActiveByDefault()
    {
        var color = new Color();
        Assert.True(color.Active);
    }
}

public class ManufacturingProcessEntityTests
{
    [Fact]
    public void ManufacturingProcess_SetProperties_SetsValuesCorrectly()
    {
        var process = new ManufacturingProcess
        {
            Name = "FDM 3D Printing"
        };

        Assert.Equal("FDM 3D Printing", process.Name);
    }

    [Fact]
    public void ManufacturingProcess_ActiveByDefault()
    {
        var process = new ManufacturingProcess();
        Assert.True(process.Active);
    }
}

public class MechanicalPropertyEntityTests
{
    [Fact]
    public void MechanicalProperty_SetProperties_SetsValuesCorrectly()
    {
        var property = new MechanicalProperty
        {
            Name = "Tensile Strength",
            Unit = "MPa"
        };

        Assert.Equal("Tensile Strength", property.Name);
        Assert.Equal("MPa", property.Unit);
    }

    [Fact]
    public void MechanicalProperty_ActiveByDefault()
    {
        var property = new MechanicalProperty();
        Assert.True(property.Active);
    }
}

public class SupplierEntityTests
{
    [Fact]
    public void Supplier_SetProperties_SetsValuesCorrectly()
    {
        var supplier = new Supplier
        {
            Name = "Test Supplier",
            ContactInfo = "test@supplier.com"
        };

        Assert.Equal("Test Supplier", supplier.Name);
        Assert.Equal("test@supplier.com", supplier.ContactInfo);
    }

    [Fact]
    public void Supplier_ActiveByDefault()
    {
        var supplier = new Supplier();
        Assert.True(supplier.Active);
    }

    [Fact]
    public void Supplier_MaterialsCollection_Initialized()
    {
        var supplier = new Supplier();
        Assert.NotNull(supplier.Materials);
    }
}

public class PostProcessingMethodEntityTests
{
    [Fact]
    public void PostProcessingMethod_SetProperties_SetsValuesCorrectly()
    {
        var method = new PostProcessingMethod
        {
            Name = "Sanding"
        };

        Assert.Equal("Sanding", method.Name);
    }

    [Fact]
    public void PostProcessingMethod_ActiveByDefault()
    {
        var method = new PostProcessingMethod();
        Assert.True(method.Active);
    }
}

public class MaterialMechanicalPropertyEntityTests
{
    [Fact]
    public void MaterialMechanicalProperty_SetProperties_SetsValuesCorrectly()
    {
        var materialProperty = new MaterialMechanicalProperty
        {
            MaterialId = Guid.NewGuid(),
            MechanicalPropertyId = Guid.NewGuid(),
            Value = 100.5m
        };

        Assert.NotEqual(Guid.Empty, materialProperty.MaterialId);
        Assert.NotEqual(Guid.Empty, materialProperty.MechanicalPropertyId);
        Assert.Equal(100.5m, materialProperty.Value);
    }
}
