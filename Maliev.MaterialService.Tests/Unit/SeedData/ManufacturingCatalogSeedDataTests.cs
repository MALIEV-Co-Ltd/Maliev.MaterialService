using Maliev.MaterialService.Infrastructure.Data.SeedData;

namespace Maliev.MaterialService.Tests.Unit.SeedData;

public class ManufacturingCatalogSeedDataTests
{
    [Fact]
    public void GetMaterialColorLinks_IncludesPeekStandardColors()
    {
        var peek = ManufacturingCatalogSeedData.GetMaterials().Single(material => material.Code == "PEEK");
        var colors = ManufacturingCatalogSeedData.GetColors().ToDictionary(color => color.Name);
        var links = ManufacturingCatalogSeedData.GetMaterialColorLinks().ToList();

        Assert.Contains(links, link => link.MaterialId == peek.Id && link.ColorId == colors["Natural"].Id);
        Assert.Contains(links, link => link.MaterialId == peek.Id && link.ColorId == colors["Black"].Id);
    }

    [Fact]
    public void GetMaterialMechanicalPropertyLinks_IncludesPeekTechnicalProperties()
    {
        var peek = ManufacturingCatalogSeedData.GetMaterials().Single(material => material.Code == "PEEK");
        var properties = ManufacturingCatalogSeedData.GetMechanicalProperties().ToDictionary(property => property.Name);
        var links = ManufacturingCatalogSeedData.GetMaterialMechanicalPropertyLinks().ToList();

        Assert.Contains(links, link =>
            link.MaterialId == peek.Id &&
            link.MechanicalPropertyId == properties["Tensile Strength"].Id &&
            link.Value == 90m);
        Assert.Contains(links, link =>
            link.MaterialId == peek.Id &&
            link.MechanicalPropertyId == properties["Chemical Resistance"].Id &&
            link.Value == 5m);
        Assert.True(links.Count(link => link.MaterialId == peek.Id) >= 6);
    }
}
