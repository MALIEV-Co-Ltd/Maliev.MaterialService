using Maliev.MaterialService.Domain.Entities;
using Maliev.MaterialService.Infrastructure.Search;

namespace Maliev.MaterialService.Tests.Unit.Services;

public class MaterialSearchDocumentMapperTests
{
    [Fact]
    public void ToUpsertEvent_WithMaterial_MapsSearchDocument()
    {
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Name = "Aluminum 6061",
            Code = "AL-6061",
            Description = "CNC grade aluminum",
            PricePerUnit = 120.50m,
            StockLevel = 25,
            Active = true
        };

        var message = MaterialSearchDocumentMapper.ToUpsertEvent(material, DateTimeOffset.UtcNow);

        Assert.Equal("MaterialService", message.Payload.SourceService);
        Assert.Equal("material", message.Payload.ResourceType);
        Assert.Equal(material.Id.ToString(), message.Payload.ResourceId);
        Assert.Equal("Aluminum 6061", message.Payload.Title);
        Assert.Equal("material.materials.read", message.Payload.RequiredPermission);
        Assert.Contains("AL-6061", message.Payload.Keywords);
    }

    [Fact]
    public void ToDeletedEvent_WithMaterialId_MapsSearchTombstone()
    {
        var materialId = Guid.NewGuid();

        var message = MaterialSearchDocumentMapper.ToDeletedEvent(materialId, DateTimeOffset.UtcNow);

        Assert.Equal("MaterialService", message.Payload.SourceService);
        Assert.Equal("material", message.Payload.ResourceType);
        Assert.Equal(materialId.ToString(), message.Payload.ResourceId);
    }
}
