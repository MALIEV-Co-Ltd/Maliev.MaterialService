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
}
