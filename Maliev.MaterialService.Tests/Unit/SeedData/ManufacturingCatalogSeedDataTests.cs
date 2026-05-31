using Maliev.MaterialService.Infrastructure.Data.SeedData;

namespace Maliev.MaterialService.Tests.Unit.SeedData;

public class ManufacturingCatalogSeedDataTests
{
    // ── Material–Finish compatibility rules ────────────────────────────────────

    [Fact]
    public void AnodizeFinishes_NotOnPomOrPeek()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var anodizeCodes = new[] { "ANODIZE_CLEAR", "ANODIZE_BLACK", "ANODIZE_HARD" };
        var plasticCodes = new[] { "DELRIN", "PEEK" };

        foreach (var plasticCode in plasticCodes)
        {
            var matId = materials[plasticCode].Id;
            foreach (var finishCode in anodizeCodes)
            {
                var finishId = finishes[finishCode].Id;
                Assert.False(
                    links.Any(l => l.MaterialId == matId && l.FinishId == finishId),
                    $"Anodize ({finishCode}) must not be offered for {plasticCode} — anodizing requires electrochemical oxidation of metal.");
            }
        }
    }

    [Fact]
    public void MirrorPolish_NotOnPomOrPeek()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var mirrorId = finishes["MIRROR_POLISH"].Id;

        Assert.False(
            links.Any(l => l.MaterialId == materials["DELRIN"].Id && l.FinishId == mirrorId),
            "Mirror Polish must not be offered for POM/Delrin — semi-crystalline polymer cannot achieve specular metal-grade surface.");
        Assert.False(
            links.Any(l => l.MaterialId == materials["PEEK"].Id && l.FinishId == mirrorId),
            "Mirror Polish must not be offered for PEEK.");
    }

    [Fact]
    public void ElectropolishFinish_NotOnCncPolymers()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var epId = finishes["ELECTROPOLISH"].Id;
        var polymers = new[] { "DELRIN", "PEEK", "PLA", "PETG", "ABS", "PA12", "TPU95A", "ASA", "PC", "CF_PETG" };

        foreach (var code in polymers)
        {
            var matId = materials[code].Id;
            Assert.False(
                links.Any(l => l.MaterialId == matId && l.FinishId == epId),
                $"Electropolish must not be offered for polymer {code}.");
        }
    }

    [Fact]
    public void DyeFinish_OfferedForSlsAndMjfNylon()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var dyeId = finishes["DYE"].Id;
        var dyeable = new[] { "PA12_SLS", "PA11_SLS", "PA12_MJF" };

        foreach (var code in dyeable)
        {
            var matId = materials[code].Id;
            Assert.True(
                links.Any(l => l.MaterialId == matId && l.FinishId == dyeId),
                $"Dye must be offered for {code} — SLS/MJF nylon dyeing is industry-standard.");
        }
    }

    [Fact]
    public void DyeFinish_NotOnGlassFilledNylon()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var dyeId = finishes["DYE"].Id;
        var glassFilled = new[] { "PA12GF_SLS", "PA12GB_MJF" };

        foreach (var code in glassFilled)
        {
            var matId = materials[code].Id;
            Assert.False(
                links.Any(l => l.MaterialId == matId && l.FinishId == dyeId),
                $"Dye must not be offered for glass-filled {code} — glass disrupts dye uptake.");
        }
    }

    [Fact]
    public void DyeFinish_NameIsDyed_NotSlaSuffix()
    {
        var dye = ManufacturingCatalogSeedData.GetSurfaceFinishes().Single(f => f.Code == "DYE");
        Assert.Equal("Dyed", dye.Name);
    }

    [Fact]
    public void VaporSmooth_OfferedForAbsAndAsa_NotForPla()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var vsId = finishes["VAPOR_SMOOTH"].Id;

        // ABS and ASA support acetone vapor smoothing
        var absId = materials["ABS"].Id;
        var asaId = materials["ASA"].Id;
        Assert.Contains(links, l => l.MaterialId == absId && l.FinishId == vsId);
        Assert.Contains(links, l => l.MaterialId == asaId && l.FinishId == vsId);

        // PLA does NOT dissolve in acetone — no vapor smooth
        Assert.False(
            links.Any(l => l.MaterialId == materials["PLA"].Id && l.FinishId == vsId),
            "Vapor Smooth must not be offered for PLA — PLA is not acetone-soluble.");
    }

    [Fact]
    public void InjectionMoldingMaterials_HaveAsMoldedFinish()
    {
        var materials = ManufacturingCatalogSeedData.GetMaterials().ToDictionary(m => m.Code);
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var links = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();

        var asMoldedId = finishes["AS_MOLDED"].Id;
        var imMaterials = new[] { "PP_IM", "ABS_IM", "PCABS_IM", "PA66_IM", "TPE_IM" };

        foreach (var code in imMaterials)
        {
            var matId = materials[code].Id;
            Assert.True(
                links.Any(l => l.MaterialId == matId && l.FinishId == asMoldedId),
                $"As-molded must be offered for injection molding material {code}.");
        }
    }

    [Fact]
    public void SlsAndMjfProcesses_IncludeDyeFinishLink()
    {
        var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToDictionary(f => f.Code);
        var dyeId = finishes["DYE"].Id;
        var processLinks = ManufacturingCatalogSeedData.GetProcessFinishLinks().ToList();

        Assert.True(
            processLinks.Any(l => l.ProcessId == ManufacturingCatalogSeedData.SlsId && l.FinishId == dyeId),
            "DYE finish must be in the SLS process–finish links.");
        Assert.True(
            processLinks.Any(l => l.ProcessId == ManufacturingCatalogSeedData.MjfId && l.FinishId == dyeId),
            "DYE finish must be in the MJF process–finish links.");
    }

    // ── Color tests ────────────────────────────────────────────────────────────

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
