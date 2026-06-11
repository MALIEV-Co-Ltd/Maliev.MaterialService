using System.Security.Cryptography;
using System.Text;
using Maliev.MaterialService.Domain.Entities;

namespace Maliev.MaterialService.Infrastructure.Data.SeedData;

/// <summary>
/// Provides static seed data for the manufacturing catalog.
/// </summary>
public static class ManufacturingCatalogSeedData
{
    private static Guid G(string code)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(code));
        var bytes = new byte[16];
        Array.Copy(hash, bytes, 16);
        bytes[6] = (byte)((bytes[6] & 0x0f) | 0x50);
        bytes[8] = (byte)((bytes[8] & 0x3f) | 0x80);
        return new Guid(bytes);
    }

    // ── Process IDs (referenced by other seed data) ───────────────────────────
    public static readonly Guid CncId = G("PROC_CNC");           // legacy — kept for backward compat
    public static readonly Guid CncMillId = G("PROC_CNC_MILL");
    public static readonly Guid CncTurnId = G("PROC_CNC_TURN");
    public static readonly Guid FdmId = G("PROC_FDM");
    public static readonly Guid SlaDlpId = G("PROC_SLA_DLP");
    public static readonly Guid SlsId = G("PROC_SLS");
    public static readonly Guid MjfId = G("PROC_MJF");
    public static readonly Guid MjId = G("PROC_MJ");
    public static readonly Guid BjId = G("PROC_BJ");
    public static readonly Guid DmlsId = G("PROC_DMLS");
    public static readonly Guid SheetMetalId = G("PROC_SHEET_METAL");
    public static readonly Guid InjectionMoldId = G("PROC_INJECTION_MOLD");

    public static IEnumerable<ManufacturingProcess> GetProcesses() =>
    [
        new() { Id = CncId, Name = "CNC Machining", Code = "CNC", Description = "Subtractive manufacturing from solid billet (legacy)", SortOrder = 10, Active = true },
        new() { Id = CncMillId, Name = "CNC Milling", Code = "CNC_MILL", Description = "Multi-axis subtractive milling from solid billet", SortOrder = 11, Active = true },
        new() { Id = CncTurnId, Name = "CNC Turning", Code = "CNC_TURN", Description = "Lathe turning for rotationally symmetric parts", SortOrder = 12, Active = true },
        new() { Id = FdmId, Name = "3D Printing (FDM)", Code = "FDM", Description = "Fused Deposition Modeling — thermoplastic filament", SortOrder = 20, Active = true },
        new() { Id = SlaDlpId, Name = "3D Printing (SLA/DLP)", Code = "SLA_DLP", Description = "Stereolithography — UV-cured resin", SortOrder = 30, Active = true },
        new() { Id = SlsId, Name = "3D Printing (SLS)", Code = "SLS", Description = "Selective Laser Sintering — powder bed nylon", SortOrder = 35, Active = true },
        new() { Id = MjfId, Name = "3D Printing (MJF)", Code = "MJF", Description = "Multi Jet Fusion — powder bed fusing agent", SortOrder = 36, Active = true },
        new() { Id = MjId, Name = "3D Printing (Material Jetting)", Code = "MJ", Description = "Material Jetting — inkjet photopolymer droplets", SortOrder = 37, Active = true },
        new() { Id = BjId, Name = "3D Printing (Binder Jetting)", Code = "BJ", Description = "Binder Jetting — powder bed with liquid binder", SortOrder = 38, Active = true },
        new() { Id = DmlsId, Name = "3D Printing (DMLS)", Code = "DMLS", Description = "Direct Metal Laser Sintering — metal powder bed fusion", SortOrder = 39, Active = true },
        new() { Id = SheetMetalId, Name = "Sheet Metal Fabrication", Code = "SHEET_METAL", Description = "Laser cut, bent, and formed sheet", SortOrder = 40, Active = true },
        new() { Id = InjectionMoldId, Name = "Injection Molding", Code = "INJECTION_MOLD", Description = "High-volume thermoplastic injection", SortOrder = 50, Active = true },
    ];

    public static IEnumerable<Material> GetMaterials() =>
    [
        // CNC — Metals
        new() { Id = G("MAT_AL6061"), Name = "Aluminum 6061-T6", Code = "AL6061", Category = "Metal", DensityGCm3 = 2.70m, MachinabilityRating = 4.0m, Description = "Most common CNC aluminum alloy. Excellent machinability and corrosion resistance.", SortOrder = 10, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_AL7075"), Name = "Aluminum 7075-T6", Code = "AL7075", Category = "Metal", DensityGCm3 = 2.81m, MachinabilityRating = 3.5m, Description = "High-strength aluminum used in aerospace applications.", SortOrder = 20, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_SS304"), Name = "Stainless Steel 304", Code = "SS304", Category = "Metal", DensityGCm3 = 8.00m, MachinabilityRating = 2.0m, Description = "General-purpose austenitic stainless steel. Good corrosion resistance.", SortOrder = 30, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_SS316L"), Name = "Stainless Steel 316L", Code = "SS316L", Category = "Metal", DensityGCm3 = 8.00m, MachinabilityRating = 1.8m, Description = "Marine-grade stainless steel with superior corrosion resistance.", SortOrder = 40, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_BRASS_C360"), Name = "Brass C360", Code = "BRASS_C360", Category = "Metal", DensityGCm3 = 8.49m, MachinabilityRating = 5.0m, Description = "Free-machining brass. Excellent machinability for precision parts.", SortOrder = 50, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_COPPER_C110"), Name = "Copper C110", Code = "COPPER_C110", Category = "Metal", DensityGCm3 = 8.94m, MachinabilityRating = 2.5m, Description = "High-conductivity copper for electrical components.", SortOrder = 60, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_TI6AL4V"), Name = "Titanium Ti-6Al-4V", Code = "TI6AL4V", Category = "Metal", DensityGCm3 = 4.43m, MachinabilityRating = 1.5m, Description = "Aerospace-grade titanium alloy. High strength-to-weight ratio.", SortOrder = 70, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // CNC — Engineering Polymers
        new() { Id = G("MAT_PEEK"), Name = "PEEK", Code = "PEEK", Category = "Polymer", DensityGCm3 = 1.31m, MachinabilityRating = 3.0m, Description = "High-performance engineering polymer. Excellent chemical and temperature resistance.", SortOrder = 80, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_DELRIN"), Name = "Delrin / POM", Code = "DELRIN", Category = "Polymer", DensityGCm3 = 1.41m, MachinabilityRating = 4.5m, Description = "Acetal homopolymer. Low friction, high stiffness, easy to machine.", SortOrder = 90, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_ACRYLIC_CLEAR"), Name = "Clear Acrylic / PMMA", Code = "ACRYLIC_CLEAR", Category = "Polymer", DensityGCm3 = 1.18m, MachinabilityRating = 3.8m, Description = "Optically clear PMMA acrylic for CNC-machined transparent covers, light pipes, and display parts.", SortOrder = 95, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // FDM Materials
        new() { Id = G("MAT_PLA"), Name = "PLA", Code = "PLA", Category = "Polymer", DensityGCm3 = 1.24m, MachinabilityRating = null, Description = "Biodegradable thermoplastic. Easy to print, good for prototypes.", SortOrder = 100, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PETG"), Name = "PETG", Code = "PETG", Category = "Polymer", DensityGCm3 = 1.27m, MachinabilityRating = null, Description = "Durable, slightly flexible. Good chemical resistance.", SortOrder = 110, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PETG_CLEAR"), Name = "Clear PETG", Code = "PETG_CLEAR", Category = "Polymer", DensityGCm3 = 1.27m, MachinabilityRating = null, Description = "Translucent clear PETG filament for printed parts needing light transmission or internal visibility.", SortOrder = 115, Active = false, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_ABS"), Name = "ABS", Code = "ABS", Category = "Polymer", DensityGCm3 = 1.04m, MachinabilityRating = null, Description = "Impact-resistant thermoplastic. Good for functional parts.", SortOrder = 120, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PA12"), Name = "Nylon PA12", Code = "PA12", Category = "Polymer", DensityGCm3 = 1.02m, MachinabilityRating = null, Description = "Strong, flexible nylon for snap-fits and living hinges.", SortOrder = 130, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_TPU95A"), Name = "TPU 95A", Code = "TPU95A", Category = "Polymer", DensityGCm3 = 1.21m, MachinabilityRating = null, Description = "Flexible elastomer for gaskets, grips, and wearables.", SortOrder = 140, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_ASA"), Name = "ASA", Code = "ASA", Category = "Polymer", DensityGCm3 = 1.07m, MachinabilityRating = null, Description = "UV-resistant ABS alternative for outdoor applications.", SortOrder = 150, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PC"), Name = "Polycarbonate", Code = "PC", Category = "Polymer", DensityGCm3 = 1.20m, MachinabilityRating = null, Description = "High-impact, heat-resistant thermoplastic.", SortOrder = 160, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_CF_PETG"), Name = "Carbon Fiber PETG", Code = "CF_PETG", Category = "Polymer", DensityGCm3 = 1.35m, MachinabilityRating = null, Description = "PETG reinforced with carbon fibers for increased stiffness.", SortOrder = 170, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // SLA/DLP Resins
        new() { Id = G("MAT_STD_RESIN"), Name = "Standard Resin", Code = "STD_RESIN", Category = "Resin", DensityGCm3 = 1.10m, MachinabilityRating = null, Description = "General-purpose photopolymer resin for detailed prototypes.", SortOrder = 180, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_CLEAR_RESIN"), Name = "Clear Resin", Code = "CLEAR_RESIN", Category = "Resin", DensityGCm3 = 1.10m, MachinabilityRating = null, Description = "Transparent photopolymer resin for SLA/DLP prototypes requiring light transmission or internal visibility.", SortOrder = 185, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_TOUGH_RESIN"), Name = "Tough Resin", Code = "TOUGH_RESIN", Category = "Resin", DensityGCm3 = 1.15m, MachinabilityRating = null, Description = "Impact-resistant resin simulating ABS-like properties.", SortOrder = 190, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_FLEX_RESIN"), Name = "Flexible Resin", Code = "FLEX_RESIN", Category = "Resin", DensityGCm3 = 1.05m, MachinabilityRating = null, Description = "Rubber-like resin for flexible components.", SortOrder = 200, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_CAST_RESIN"), Name = "Castable Resin", Code = "CAST_RESIN", Category = "Resin", DensityGCm3 = 1.08m, MachinabilityRating = null, Description = "Burns out cleanly for investment casting applications.", SortOrder = 210, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_HT_RESIN"), Name = "High-Temp Resin", Code = "HT_RESIN", Category = "Resin", DensityGCm3 = 1.12m, MachinabilityRating = null, Description = "Heat-deflection resistant resin for high-temperature applications.", SortOrder = 220, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // SLS Materials
        new() { Id = G("MAT_PA12_SLS"), Name = "Nylon PA12 (SLS)", Code = "PA12_SLS", Category = "Polymer", DensityGCm3 = 0.93m, MachinabilityRating = null, Description = "SLS-grade PA12. Strong, flexible, fine detail. Industry workhorse for powder-bed nylon.", SortOrder = 280, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PA11_SLS"), Name = "Nylon PA11 (SLS)", Code = "PA11_SLS", Category = "Polymer", DensityGCm3 = 1.01m, MachinabilityRating = null, Description = "Bio-based PA11. Higher elongation and impact resistance than PA12.", SortOrder = 290, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PA12GF_SLS"), Name = "Glass-Filled PA12 (SLS)", Code = "PA12GF_SLS", Category = "Polymer", DensityGCm3 = 1.22m, MachinabilityRating = null, Description = "PA12 reinforced with glass spheres. Higher stiffness and heat resistance than standard PA12.", SortOrder = 300, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // MJF Materials
        new() { Id = G("MAT_PA12_MJF"), Name = "Nylon PA12 (MJF)", Code = "PA12_MJF", Category = "Polymer", DensityGCm3 = 1.01m, MachinabilityRating = null, Description = "HP MJF PA12. Isotropic mechanical properties, fine detail, natural grey finish.", SortOrder = 310, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PA12GB_MJF"), Name = "PA12 Glass Bead (MJF)", Code = "PA12GB_MJF", Category = "Polymer", DensityGCm3 = 1.22m, MachinabilityRating = null, Description = "HP MJF PA12 with glass beads. Increased stiffness and dimensional stability.", SortOrder = 320, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // Material Jetting Materials
        new() { Id = G("MAT_VEROWHITE"), Name = "VeroWhite (MJ)", Code = "VEROWHITE", Category = "Photopolymer", DensityGCm3 = 1.17m, MachinabilityRating = null, Description = "Rigid opaque white photopolymer. High accuracy and smooth surface for visual prototypes.", SortOrder = 330, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_VEROBLACK"), Name = "VeroBlack (MJ)", Code = "VEROBLACK", Category = "Photopolymer", DensityGCm3 = 1.17m, MachinabilityRating = null, Description = "Rigid opaque black photopolymer. Same properties as VeroWhite, matte black finish.", SortOrder = 340, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_TANGOPLUS"), Name = "TangoPlus (MJ)", Code = "TANGOPLUS", Category = "Photopolymer", DensityGCm3 = 1.12m, MachinabilityRating = null, Description = "Rubber-like flexible photopolymer (Shore A 26–28). For grips, seals, and flexible joints.", SortOrder = 350, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // Binder Jetting Materials
        new() { Id = G("MAT_SS316L_BJ"), Name = "Stainless Steel 316L (BJ)", Code = "SS316L_BJ", Category = "Metal", DensityGCm3 = 7.90m, MachinabilityRating = null, Description = "Binder-jetted 316L SS, sintered. Fully dense metal with corrosion resistance.", SortOrder = 360, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_BRONZE_BJ"), Name = "Bronze (BJ)", Code = "BRONZE_BJ", Category = "Metal", DensityGCm3 = 8.70m, MachinabilityRating = null, Description = "Binder-jetted bronze. Good thermal conductivity, golden appearance.", SortOrder = 370, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_SAND_BJ"), Name = "Silica Sand (BJ)", Code = "SAND_BJ", Category = "Sand", DensityGCm3 = 1.50m, MachinabilityRating = null, Description = "Binder-jetted sand molds for metal casting. High thermal stability.", SortOrder = 380, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // DMLS Materials
        new() { Id = G("MAT_TI6AL4V_DMLS"), Name = "Ti-6Al-4V (DMLS)", Code = "TI6AL4V_DMLS", Category = "Metal", DensityGCm3 = 4.42m, MachinabilityRating = null, Description = "DMLS titanium alloy. Aerospace grade, excellent strength-to-weight ratio.", SortOrder = 390, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_ALSI10MG"), Name = "AlSi10Mg (DMLS)", Code = "ALSI10MG", Category = "Metal", DensityGCm3 = 2.67m, MachinabilityRating = null, Description = "DMLS aluminum alloy. Lightweight, good thermal conductivity, complex geometries.", SortOrder = 400, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_IN718"), Name = "Inconel 718 (DMLS)", Code = "IN718", Category = "Metal", DensityGCm3 = 8.19m, MachinabilityRating = null, Description = "DMLS nickel superalloy. Excellent high-temperature performance to 700°C.", SortOrder = 410, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_174PH"), Name = "17-4PH Stainless (DMLS)", Code = "174PH", Category = "Metal", DensityGCm3 = 7.78m, MachinabilityRating = null, Description = "DMLS precipitation-hardening stainless steel. High strength and corrosion resistance.", SortOrder = 420, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // Sheet Metal Materials
        new() { Id = G("MAT_MILD_STEEL"), Name = "Mild Steel (CR/HR)", Code = "MILD_STEEL", Category = "Sheet", DensityGCm3 = 7.85m, MachinabilityRating = null, Description = "Low-carbon steel sheet. 1.0–3.0mm thickness range.", SortOrder = 230, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_SS304_SHEET"), Name = "Stainless Steel 304 Sheet", Code = "SS304_SHEET", Category = "Sheet", DensityGCm3 = 8.00m, MachinabilityRating = null, Description = "Stainless steel sheet. 0.5–3.0mm thickness range.", SortOrder = 240, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_AL5052"), Name = "Aluminum 5052 Sheet", Code = "AL5052", Category = "Sheet", DensityGCm3 = 2.68m, MachinabilityRating = null, Description = "Aluminum sheet with good formability. 0.5–5.0mm thickness range.", SortOrder = 250, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_COPPER_SHEET"), Name = "Copper Sheet", Code = "COPPER_SHEET", Category = "Sheet", DensityGCm3 = 8.94m, MachinabilityRating = null, Description = "Copper sheet for electrical and thermal applications. 0.5–2.0mm.", SortOrder = 260, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_BRASS_SHEET"), Name = "Brass Sheet", Code = "BRASS_SHEET", Category = "Sheet", DensityGCm3 = 8.49m, MachinabilityRating = null, Description = "Brass sheet for decorative and precision components. 0.5–2.0mm.", SortOrder = 270, Active = true, PricePerUnit = 0, StockLevel = 0 },
        // Injection Molding Materials
        new() { Id = G("MAT_PP_IM"), Name = "Polypropylene (PP)", Code = "PP_IM", Category = "Polymer", DensityGCm3 = 0.91m, MachinabilityRating = null, Description = "Lightweight, chemical-resistant thermoplastic. Excellent for living hinges.", SortOrder = 430, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_ABS_IM"), Name = "ABS (Injection)", Code = "ABS_IM", Category = "Polymer", DensityGCm3 = 1.05m, MachinabilityRating = null, Description = "Impact-resistant, easy to paint and glue. General-purpose engineering plastic.", SortOrder = 440, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PCABS_IM"), Name = "PC/ABS Blend", Code = "PCABS_IM", Category = "Polymer", DensityGCm3 = 1.12m, MachinabilityRating = null, Description = "High-impact blend combining PC strength with ABS processability.", SortOrder = 450, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_PA66_IM"), Name = "Nylon PA66", Code = "PA66_IM", Category = "Polymer", DensityGCm3 = 1.14m, MachinabilityRating = null, Description = "High-strength nylon for mechanical components, gears, and wear-resistant parts.", SortOrder = 460, Active = true, PricePerUnit = 0, StockLevel = 0 },
        new() { Id = G("MAT_TPE_IM"), Name = "TPE (Overmold)", Code = "TPE_IM", Category = "Polymer", DensityGCm3 = 1.10m, MachinabilityRating = null, Description = "Thermoplastic elastomer for soft-touch grips, seals, and overmolding applications.", SortOrder = 470, Active = true, PricePerUnit = 0, StockLevel = 0 },
    ];

    public static IEnumerable<Color> GetColors() =>
    [
        new() { Id = G("COLOR_NATURAL"), Name = "Natural", HexCode = "#d9c7a4", Active = true },
        new() { Id = G("COLOR_BLACK"), Name = "Black", HexCode = "#171717", Active = true },
        new() { Id = G("COLOR_WHITE"), Name = "White", HexCode = "#f5f5f5", Active = true },
        new() { Id = G("COLOR_GREY"), Name = "Grey", HexCode = "#8a8f98", Active = true },
        new() { Id = G("COLOR_CLEAR"), Name = "Clear", HexCode = "#dceeff", Active = true },
        new() { Id = G("COLOR_SILVER"), Name = "Silver", HexCode = "#c0c0c0", Active = true },
        new() { Id = G("COLOR_GOLD"), Name = "Gold", HexCode = "#d4a017", Active = true },
        new() { Id = G("COLOR_COPPER"), Name = "Copper", HexCode = "#b87333", Active = true },
        new() { Id = G("COLOR_BLUE"), Name = "Blue", HexCode = "#2563eb", Active = true },
        new() { Id = G("COLOR_RED"), Name = "Red", HexCode = "#dc2626", Active = true },
        new() { Id = G("COLOR_GREEN"), Name = "Green", HexCode = "#16a34a", Active = true },
        new() { Id = G("COLOR_YELLOW"), Name = "Yellow", HexCode = "#facc15", Active = true },
        new() { Id = G("COLOR_ORANGE"), Name = "Orange", HexCode = "#f97316", Active = true },
        new() { Id = G("COLOR_AMBER"), Name = "Amber", HexCode = "#f59e0b", Active = true },
    ];

    public static IEnumerable<MechanicalProperty> GetMechanicalProperties() =>
    [
        new() { Id = G("PROP_DENSITY"), Name = "Density", Unit = "g/cm3", Active = true },
        new() { Id = G("PROP_TENSILE_STRENGTH"), Name = "Tensile Strength", Unit = "MPa", Active = true },
        new() { Id = G("PROP_ELASTIC_MODULUS"), Name = "Elastic Modulus", Unit = "GPa", Active = true },
        new() { Id = G("PROP_HEAT_DEFLECTION_TEMP"), Name = "Heat Deflection Temp", Unit = "deg C", Active = true },
        new() { Id = G("PROP_CONTINUOUS_USE_TEMP"), Name = "Continuous Use Temp", Unit = "deg C", Active = true },
        new() { Id = G("PROP_CHEMICAL_RESISTANCE"), Name = "Chemical Resistance", Unit = "1-5", Active = true },
        new() { Id = G("PROP_MACHINABILITY"), Name = "Machinability", Unit = "1-5", Active = true },
    ];

    public static IEnumerable<(Guid MaterialId, Guid ColorId)> GetMaterialColorLinks() =>
    [
        // Metals
        (G("MAT_AL6061"), G("COLOR_SILVER")), (G("MAT_AL7075"), G("COLOR_SILVER")),
        (G("MAT_SS304"), G("COLOR_SILVER")), (G("MAT_SS316L"), G("COLOR_SILVER")),
        (G("MAT_BRASS_C360"), G("COLOR_GOLD")), (G("MAT_COPPER_C110"), G("COLOR_COPPER")),
        (G("MAT_TI6AL4V"), G("COLOR_GREY")),
        // CNC polymers
        (G("MAT_PEEK"), G("COLOR_NATURAL")), (G("MAT_PEEK"), G("COLOR_BLACK")),
        (G("MAT_DELRIN"), G("COLOR_WHITE")), (G("MAT_DELRIN"), G("COLOR_BLACK")),
        (G("MAT_ACRYLIC_CLEAR"), G("COLOR_CLEAR")),
        // FDM polymers — extended colors based on commercially available filaments
        (G("MAT_PLA"), G("COLOR_WHITE")), (G("MAT_PLA"), G("COLOR_BLACK")), (G("MAT_PLA"), G("COLOR_GREY")),
        (G("MAT_PLA"), G("COLOR_RED")), (G("MAT_PLA"), G("COLOR_BLUE")), (G("MAT_PLA"), G("COLOR_GREEN")),
        (G("MAT_PLA"), G("COLOR_YELLOW")), (G("MAT_PLA"), G("COLOR_ORANGE")),
        (G("MAT_PETG"), G("COLOR_CLEAR")), (G("MAT_PETG_CLEAR"), G("COLOR_CLEAR")),
        (G("MAT_PETG"), G("COLOR_BLACK")), (G("MAT_PETG"), G("COLOR_WHITE")),
        (G("MAT_PETG"), G("COLOR_GREY")),
        (G("MAT_ABS"), G("COLOR_BLACK")), (G("MAT_ABS"), G("COLOR_WHITE")), (G("MAT_ABS"), G("COLOR_GREY")),
        (G("MAT_PA12"), G("COLOR_NATURAL")), (G("MAT_PA12"), G("COLOR_BLACK")),
        (G("MAT_TPU95A"), G("COLOR_BLACK")), (G("MAT_TPU95A"), G("COLOR_WHITE")),
        (G("MAT_ASA"), G("COLOR_BLACK")), (G("MAT_ASA"), G("COLOR_WHITE")), (G("MAT_ASA"), G("COLOR_GREY")),
        (G("MAT_PC"), G("COLOR_CLEAR")), (G("MAT_PC"), G("COLOR_BLACK")),
        (G("MAT_CF_PETG"), G("COLOR_BLACK")),
        // Resins and powder-bed polymers
        (G("MAT_STD_RESIN"), G("COLOR_GREY")),
        (G("MAT_CLEAR_RESIN"), G("COLOR_CLEAR")),
        (G("MAT_TOUGH_RESIN"), G("COLOR_GREY")), (G("MAT_FLEX_RESIN"), G("COLOR_BLACK")),
        (G("MAT_CAST_RESIN"), G("COLOR_AMBER")), (G("MAT_HT_RESIN"), G("COLOR_AMBER")),
        (G("MAT_PA12_SLS"), G("COLOR_NATURAL")), (G("MAT_PA12_SLS"), G("COLOR_BLACK")), (G("MAT_PA12_SLS"), G("COLOR_GREY")),
        (G("MAT_PA11_SLS"), G("COLOR_NATURAL")), (G("MAT_PA11_SLS"), G("COLOR_BLACK")), (G("MAT_PA11_SLS"), G("COLOR_GREY")),
        (G("MAT_PA12GF_SLS"), G("COLOR_GREY")),
        (G("MAT_PA12_MJF"), G("COLOR_GREY")), (G("MAT_PA12_MJF"), G("COLOR_BLACK")),
        (G("MAT_PA12GB_MJF"), G("COLOR_GREY")),
        // Material jetting, binder jetting, DMLS, sheet
        (G("MAT_VEROWHITE"), G("COLOR_WHITE")), (G("MAT_VEROBLACK"), G("COLOR_BLACK")),
        (G("MAT_TANGOPLUS"), G("COLOR_CLEAR")), (G("MAT_SS316L_BJ"), G("COLOR_SILVER")),
        (G("MAT_BRONZE_BJ"), G("COLOR_GOLD")), (G("MAT_SAND_BJ"), G("COLOR_NATURAL")),
        (G("MAT_TI6AL4V_DMLS"), G("COLOR_GREY")), (G("MAT_ALSI10MG"), G("COLOR_SILVER")),
        (G("MAT_IN718"), G("COLOR_SILVER")), (G("MAT_174PH"), G("COLOR_SILVER")),
        (G("MAT_MILD_STEEL"), G("COLOR_GREY")), (G("MAT_SS304_SHEET"), G("COLOR_SILVER")),
        (G("MAT_AL5052"), G("COLOR_SILVER")), (G("MAT_COPPER_SHEET"), G("COLOR_COPPER")),
        (G("MAT_BRASS_SHEET"), G("COLOR_GOLD")),
        // Injection Molding — masterbatch colors. Natural/clear is raw resin; black and white are universal.
        (G("MAT_PP_IM"), G("COLOR_NATURAL")), (G("MAT_PP_IM"), G("COLOR_BLACK")), (G("MAT_PP_IM"), G("COLOR_WHITE")),
        (G("MAT_ABS_IM"), G("COLOR_BLACK")), (G("MAT_ABS_IM"), G("COLOR_WHITE")), (G("MAT_ABS_IM"), G("COLOR_GREY")),
        (G("MAT_PCABS_IM"), G("COLOR_BLACK")), (G("MAT_PCABS_IM"), G("COLOR_GREY")),
        (G("MAT_PA66_IM"), G("COLOR_NATURAL")), (G("MAT_PA66_IM"), G("COLOR_BLACK")),
        (G("MAT_TPE_IM"), G("COLOR_BLACK")), (G("MAT_TPE_IM"), G("COLOR_GREY")), (G("MAT_TPE_IM"), G("COLOR_WHITE")),
    ];

    public static IEnumerable<(Guid MaterialId, Guid MechanicalPropertyId, decimal Value)> GetMaterialMechanicalPropertyLinks() =>
    [
        (G("MAT_PEEK"), G("PROP_DENSITY"), 1.31m),
        (G("MAT_PEEK"), G("PROP_TENSILE_STRENGTH"), 90m),
        (G("MAT_PEEK"), G("PROP_ELASTIC_MODULUS"), 3.6m),
        (G("MAT_PEEK"), G("PROP_HEAT_DEFLECTION_TEMP"), 152m),
        (G("MAT_PEEK"), G("PROP_CONTINUOUS_USE_TEMP"), 250m),
        (G("MAT_PEEK"), G("PROP_CHEMICAL_RESISTANCE"), 5m),
        (G("MAT_PEEK"), G("PROP_MACHINABILITY"), 3m),
    ];

    public static IEnumerable<SurfaceFinish> GetSurfaceFinishes() =>
    [
        // CNC Finishes
        new() { Id = G("SF_AS_MACHINED"), Name = "As-machined", Code = "AS_MACHINED", RaValueUm = 3.2m, AdditionalCostPercent = 0m, SortOrder = 10, Active = true },
        new() { Id = G("SF_BEAD_BLASTED"), Name = "Bead Blasted", Code = "BEAD_BLASTED", RaValueUm = 1.6m, AdditionalCostPercent = 5m, SortOrder = 20, Active = true },
        new() { Id = G("SF_ANODIZE_CLEAR"), Name = "Anodized Natural (Type II)", Code = "ANODIZE_CLEAR", RaValueUm = 1.6m, AdditionalCostPercent = 12m, SortOrder = 30, Active = true },
        new() { Id = G("SF_ANODIZE_BLACK"), Name = "Anodized Black (Type II)", Code = "ANODIZE_BLACK", RaValueUm = 1.6m, AdditionalCostPercent = 15m, SortOrder = 40, Active = true },
        new() { Id = G("SF_ANODIZE_HARD"), Name = "Hard Anodized (Type III)", Code = "ANODIZE_HARD", RaValueUm = 1.0m, AdditionalCostPercent = 25m, SortOrder = 50, Active = true },
        new() { Id = G("SF_POWDER_COAT"), Name = "Powder Coated", Code = "POWDER_COAT", RaValueUm = 2.0m, AdditionalCostPercent = 18m, SortOrder = 60, Active = true },
        new() { Id = G("SF_ELECTROPOLISH"), Name = "Electropolished", Code = "ELECTROPOLISH", RaValueUm = 0.4m, AdditionalCostPercent = 30m, SortOrder = 70, Active = true },
        new() { Id = G("SF_BRUSHED"), Name = "Brushed", Code = "BRUSHED", RaValueUm = 0.8m, AdditionalCostPercent = 8m, SortOrder = 80, Active = true },
        new() { Id = G("SF_MIRROR_POLISH"), Name = "Mirror Polish", Code = "MIRROR_POLISH", RaValueUm = 0.1m, AdditionalCostPercent = 45m, SortOrder = 90, Active = true },
        // 3D Printing Finishes
        new() { Id = G("SF_AS_PRINTED"), Name = "As-printed", Code = "AS_PRINTED", RaValueUm = null, AdditionalCostPercent = 0m, SortOrder = 100, Active = true },
        new() { Id = G("SF_SANDED"), Name = "Sanded", Code = "SANDED", RaValueUm = null, AdditionalCostPercent = 10m, SortOrder = 110, Active = true },
        new() { Id = G("SF_VAPOR_SMOOTH"), Name = "Vapor Smoothed", Code = "VAPOR_SMOOTH", RaValueUm = null, AdditionalCostPercent = 20m, SortOrder = 120, Active = true },
        new() { Id = G("SF_PAINTED"), Name = "Painted", Code = "PAINTED", RaValueUm = null, AdditionalCostPercent = 25m, SortOrder = 130, Active = true },
        new() { Id = G("SF_DYE"), Name = "Dyed", Code = "DYE", RaValueUm = null, AdditionalCostPercent = 12m, SortOrder = 101, Active = true },
        // Injection Molding Finishes
        new() { Id = G("SF_AS_MOLDED"), Name = "As-molded", Code = "AS_MOLDED", RaValueUm = null, AdditionalCostPercent = 0m, SortOrder = 145, Active = true },
        // Sheet Metal Finishes
        new() { Id = G("SF_DEBURR"), Name = "Deburred", Code = "DEBURR", RaValueUm = null, AdditionalCostPercent = 0m, SortOrder = 150, Active = true },
        new() { Id = G("SF_POWDER_COAT_SM"), Name = "Powder Coated (Sheet Metal)", Code = "POWDER_COAT_SM", RaValueUm = 2.0m, AdditionalCostPercent = 18m, SortOrder = 160, Active = true },
        new() { Id = G("SF_ZINC_PLATE"), Name = "Zinc Plated", Code = "ZINC_PLATE", RaValueUm = null, AdditionalCostPercent = 15m, SortOrder = 170, Active = true },
        new() { Id = G("SF_GALVANIZE"), Name = "Hot-dip Galvanized", Code = "GALVANIZE", RaValueUm = null, AdditionalCostPercent = 22m, SortOrder = 180, Active = true },
    ];

    public static IEnumerable<ToleranceClass> GetToleranceClasses() =>
    [
        // ISO 2768-1 General Tolerances
        new() { Id = G("TOL_ISO2768_F"), Name = "Fine (ISO 2768-f)", Code = "ISO2768_F", IsoStandard = "ISO 2768-1", Grade = "f", ToleranceRange = "±0.05mm (0.5–6mm), ±0.1mm (6–30mm)", AdditionalCostPercent = 25m, SortOrder = 10, Active = true },
        new() { Id = G("TOL_ISO2768_M"), Name = "Medium (ISO 2768-m)", Code = "ISO2768_M", IsoStandard = "ISO 2768-1", Grade = "m", ToleranceRange = "±0.1mm (0.5–6mm), ±0.2mm (6–30mm)", AdditionalCostPercent = 10m, SortOrder = 20, Active = true },
        new() { Id = G("TOL_ISO2768_C"), Name = "Coarse (ISO 2768-c)", Code = "ISO2768_C", IsoStandard = "ISO 2768-1", Grade = "c", ToleranceRange = "±0.2mm (0.5–6mm), ±0.5mm (6–30mm)", AdditionalCostPercent = 0m, SortOrder = 30, Active = true },
        new() { Id = G("TOL_ISO2768_V"), Name = "Very Coarse (ISO 2768-v)", Code = "ISO2768_V", IsoStandard = "ISO 2768-1", Grade = "v", ToleranceRange = "±0.5mm (0.5–6mm), ±1.0mm (6–30mm)", AdditionalCostPercent = 0m, SortOrder = 40, Active = true },
        // CNC IT Grades (ISO 286)
        new() { Id = G("TOL_IT6"), Name = "IT6 Precision", Code = "IT6", IsoStandard = "ISO 286-1", Grade = "IT6", ToleranceRange = "±0.008mm (6–10mm)", AdditionalCostPercent = 60m, SortOrder = 50, Active = true },
        new() { Id = G("TOL_IT7"), Name = "IT7 Precision", Code = "IT7", IsoStandard = "ISO 286-1", Grade = "IT7", ToleranceRange = "±0.012mm (6–10mm)", AdditionalCostPercent = 40m, SortOrder = 60, Active = true },
        new() { Id = G("TOL_IT8"), Name = "IT8 Standard", Code = "IT8", IsoStandard = "ISO 286-1", Grade = "IT8", ToleranceRange = "±0.018mm (6–10mm)", AdditionalCostPercent = 20m, SortOrder = 70, Active = true },
        // FDM Tolerances
        new() { Id = G("TOL_FDM_STD"), Name = "FDM Standard", Code = "FDM_STD", IsoStandard = "FDM", Grade = "Standard", ToleranceRange = "±0.3mm", AdditionalCostPercent = 0m, SortOrder = 80, Active = true },
        new() { Id = G("TOL_FDM_FINE"), Name = "FDM Fine", Code = "FDM_FINE", IsoStandard = "FDM", Grade = "Fine", ToleranceRange = "±0.15mm", AdditionalCostPercent = 15m, SortOrder = 90, Active = true },
        // SLA/DLP Tolerances
        new() { Id = G("TOL_SLA_STD"), Name = "SLA Standard", Code = "SLA_STD", IsoStandard = "SLA", Grade = "Standard", ToleranceRange = "±0.1mm", AdditionalCostPercent = 0m, SortOrder = 100, Active = true },
        new() { Id = G("TOL_SLA_FINE"), Name = "SLA Fine", Code = "SLA_FINE", IsoStandard = "SLA", Grade = "Fine", ToleranceRange = "±0.05mm", AdditionalCostPercent = 20m, SortOrder = 110, Active = true },
        // SLS Tolerances
        new() { Id = G("TOL_SLS_STD"), Name = "SLS Standard", Code = "SLS_STD", IsoStandard = "SLS", Grade = "Standard", ToleranceRange = "±0.3mm", AdditionalCostPercent = 0m, SortOrder = 120, Active = true },
        // MJF Tolerances
        new() { Id = G("TOL_MJF_STD"), Name = "MJF Standard", Code = "MJF_STD", IsoStandard = "MJF", Grade = "Standard", ToleranceRange = "±0.3mm", AdditionalCostPercent = 0m, SortOrder = 130, Active = true },
        // Material Jetting Tolerances
        new() { Id = G("TOL_MJ_STD"), Name = "Material Jetting Standard", Code = "MJ_STD", IsoStandard = "MJ", Grade = "Standard", ToleranceRange = "±0.1mm", AdditionalCostPercent = 0m, SortOrder = 140, Active = true },
        // Binder Jetting Tolerances
        new() { Id = G("TOL_BJ_STD"), Name = "Binder Jetting Standard", Code = "BJ_STD", IsoStandard = "BJ", Grade = "Standard", ToleranceRange = "±0.2mm (metal) / ±0.3mm (sand)", AdditionalCostPercent = 0m, SortOrder = 150, Active = true },
        // DMLS Tolerances
        new() { Id = G("TOL_DMLS_STD"), Name = "DMLS Standard", Code = "DMLS_STD", IsoStandard = "DMLS", Grade = "Standard", ToleranceRange = "±0.1mm", AdditionalCostPercent = 0m, SortOrder = 160, Active = true },
        new() { Id = G("TOL_DMLS_FINE"), Name = "DMLS Fine", Code = "DMLS_FINE", IsoStandard = "DMLS", Grade = "Fine", ToleranceRange = "±0.05mm (post-machined critical features)", AdditionalCostPercent = 30m, SortOrder = 170, Active = true },
        // Injection Molding Tolerances — typical achievable range per DFM guidelines
        new() { Id = G("TOL_IM_STD"), Name = "IM Standard", Code = "IM_STD", IsoStandard = "SPI", Grade = "B", ToleranceRange = "±0.1mm (critical) / ±0.3mm (non-critical)", AdditionalCostPercent = 0m, SortOrder = 180, Active = true },
        new() { Id = G("TOL_IM_FINE"), Name = "IM Fine (tight mold)", Code = "IM_FINE", IsoStandard = "SPI", Grade = "A", ToleranceRange = "±0.05mm (requires precision tooling)", AdditionalCostPercent = 40m, SortOrder = 190, Active = true },
    ];

    public static IEnumerable<ProcessConfigOption> GetProcessConfigOptions() =>
    [
        // ── CNC Machining ──────────────────────────────────────────────────────
        new() { Id = G("OPT_CNC_RA"), ManufacturingProcessId = CncId, ConfigKey = "surface_roughness_ra", Label = "Surface Roughness (Ra)", ConfigType = "dropdown", DefaultValue = "3.2", OptionsJson = """["0.4","0.8","1.6","3.2","6.3"]""", Unit = "μm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNC_TAP"), ManufacturingProcessId = CncId, ConfigKey = "tap_thread_holes", Label = "Tap/Thread Mill Holes", ConfigType = "toggle", DefaultValue = "false", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNC_THREAD_SPEC"), ManufacturingProcessId = CncId, ConfigKey = "thread_spec", Label = "Thread Specification", ConfigType = "text", DefaultValue = null, HelpText = "e.g. M6×1.0, 1/4-20 UNC", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNC_INSPECTION"), ManufacturingProcessId = CncId, ConfigKey = "inspection_type", Label = "Inspection Type", ConfigType = "dropdown", DefaultValue = "VISUAL", OptionsJson = """["VISUAL","CMM","FIRST_ARTICLE"]""", SortOrder = 40, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNC_DEBURR"), ManufacturingProcessId = CncId, ConfigKey = "deburr_edges", Label = "Deburr All Edges", ConfigType = "toggle", DefaultValue = "true", SortOrder = 50, IsRequired = false, Active = true },

        // ── FDM 3D Printing ────────────────────────────────────────────────────
        new() { Id = G("OPT_FDM_LAYER"), ManufacturingProcessId = FdmId, ConfigKey = "layer_height_mm", Label = "Layer Height", ConfigType = "dropdown", DefaultValue = "0.2", OptionsJson = """["0.1","0.15","0.2","0.3"]""", Unit = "mm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_FDM_INFILL"), ManufacturingProcessId = FdmId, ConfigKey = "infill_percent", Label = "Infill Percentage", ConfigType = "dropdown", DefaultValue = "20", OptionsJson = """["10","15","20","50","80","100"]""", Unit = "%", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_FDM_COLOR"), ManufacturingProcessId = FdmId, ConfigKey = "color", Label = "Color", ConfigType = "dropdown", DefaultValue = "WHITE", OptionsJson = """["WHITE","BLACK","RED","BLUE","GREEN","YELLOW","ORANGE","GREY","NATURAL"]""", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_FDM_INSERT"), ManufacturingProcessId = FdmId, ConfigKey = "thread_insert", Label = "Thread Insert (Heat-set)", ConfigType = "toggle", DefaultValue = "false", SortOrder = 40, IsRequired = false, Active = true },
        new() { Id = G("OPT_FDM_TAP"), ManufacturingProcessId = FdmId, ConfigKey = "tap_holes", Label = "Tap Holes", ConfigType = "toggle", DefaultValue = "false", SortOrder = 50, IsRequired = false, Active = true },
        new() { Id = G("OPT_FDM_SUPPORT"), ManufacturingProcessId = FdmId, ConfigKey = "support_type", Label = "Support Type", ConfigType = "dropdown", DefaultValue = "AUTO", OptionsJson = """["NONE","AUTO","TREE","LINEAR"]""", SortOrder = 60, IsRequired = false, Active = true },

        // ── SLA/DLP 3D Printing ────────────────────────────────────────────────
        new() { Id = G("OPT_SLA_LAYER"), ManufacturingProcessId = SlaDlpId, ConfigKey = "layer_height_mm", Label = "Layer Height", ConfigType = "dropdown", DefaultValue = "0.05", OptionsJson = """["0.025","0.05","0.1"]""", Unit = "mm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_SLA_COLOR"), ManufacturingProcessId = SlaDlpId, ConfigKey = "color", Label = "Color", ConfigType = "dropdown", DefaultValue = "GREY", OptionsJson = """["GREY","CLEAR","BLACK","WHITE"]""", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_SLA_UV"), ManufacturingProcessId = SlaDlpId, ConfigKey = "uv_cure", Label = "UV Post-cure", ConfigType = "toggle", DefaultValue = "true", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_SLA_INSERT"), ManufacturingProcessId = SlaDlpId, ConfigKey = "thread_insert", Label = "Thread Insert", ConfigType = "toggle", DefaultValue = "false", SortOrder = 40, IsRequired = false, Active = true },

        // ── CNC Milling ────────────────────────────────────────────────────────
        new() { Id = G("OPT_CNCMILL_RA"), ManufacturingProcessId = CncMillId, ConfigKey = "surface_roughness_ra", Label = "Surface Roughness (Ra)", ConfigType = "dropdown", DefaultValue = "3.2", OptionsJson = """["0.4","0.8","1.6","3.2","6.3"]""", Unit = "μm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCMILL_TAP"), ManufacturingProcessId = CncMillId, ConfigKey = "tap_thread_holes", Label = "Tap/Thread Mill Holes", ConfigType = "toggle", DefaultValue = "false", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCMILL_THREAD_SPEC"), ManufacturingProcessId = CncMillId, ConfigKey = "thread_spec", Label = "Thread Specification", ConfigType = "text", DefaultValue = null, HelpText = "e.g. M6×1.0, 1/4-20 UNC", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCMILL_INSPECTION"), ManufacturingProcessId = CncMillId, ConfigKey = "inspection_type", Label = "Inspection Type", ConfigType = "dropdown", DefaultValue = "VISUAL", OptionsJson = """["VISUAL","CMM","FIRST_ARTICLE"]""", SortOrder = 40, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCMILL_DEBURR"), ManufacturingProcessId = CncMillId, ConfigKey = "deburr_edges", Label = "Deburr All Edges", ConfigType = "toggle", DefaultValue = "true", SortOrder = 50, IsRequired = false, Active = true },

        // ── CNC Turning ─────────────────────────────────────────────────────────
        new() { Id = G("OPT_CNCTURN_RA"), ManufacturingProcessId = CncTurnId, ConfigKey = "surface_roughness_ra", Label = "Surface Roughness (Ra)", ConfigType = "dropdown", DefaultValue = "1.6", OptionsJson = """["0.4","0.8","1.6","3.2"]""", Unit = "μm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCTURN_TAP"), ManufacturingProcessId = CncTurnId, ConfigKey = "tap_thread_holes", Label = "Thread Holes (Axial)", ConfigType = "toggle", DefaultValue = "false", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCTURN_THREAD_SPEC"), ManufacturingProcessId = CncTurnId, ConfigKey = "thread_spec", Label = "OD/ID Thread Specification", ConfigType = "text", DefaultValue = null, HelpText = "e.g. M20×2.5 OD, M12×1.75 ID", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCTURN_GROOVE"), ManufacturingProcessId = CncTurnId, ConfigKey = "groove_required", Label = "Groove / Undercut", ConfigType = "toggle", DefaultValue = "false", SortOrder = 40, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCTURN_INSPECTION"), ManufacturingProcessId = CncTurnId, ConfigKey = "inspection_type", Label = "Inspection Type", ConfigType = "dropdown", DefaultValue = "VISUAL", OptionsJson = """["VISUAL","CMM","FIRST_ARTICLE"]""", SortOrder = 50, IsRequired = false, Active = true },

        // ── SLS 3D Printing ────────────────────────────────────────────────────
        new() { Id = G("OPT_SLS_COLOR"), ManufacturingProcessId = SlsId, ConfigKey = "color", Label = "Color", ConfigType = "dropdown", DefaultValue = "NATURAL", OptionsJson = """["NATURAL","BLACK","GREY","CUSTOM_DYE"]""", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_SLS_FINISH"), ManufacturingProcessId = SlsId, ConfigKey = "post_processing", Label = "Post-Processing", ConfigType = "dropdown", DefaultValue = "BEAD_BLASTED", OptionsJson = """["BEAD_BLASTED","DYED","SANDED","VAPOR_SMOOTH","PAINTED"]""", SortOrder = 20, IsRequired = false, Active = true },

        // ── MJF 3D Printing ────────────────────────────────────────────────────
        new() { Id = G("OPT_MJF_COLOR"), ManufacturingProcessId = MjfId, ConfigKey = "color", Label = "Color", ConfigType = "dropdown", DefaultValue = "NATURAL_GREY", OptionsJson = """["NATURAL_GREY","BLACK"]""", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_MJF_FINISH"), ManufacturingProcessId = MjfId, ConfigKey = "post_processing", Label = "Post-Processing", ConfigType = "dropdown", DefaultValue = "AS_PRINTED", OptionsJson = """["AS_PRINTED","TUMBLED","DYED","PAINTED"]""", SortOrder = 20, IsRequired = false, Active = true },

        // ── Material Jetting ───────────────────────────────────────────────────
        new() { Id = G("OPT_MJ_LAYER"), ManufacturingProcessId = MjId, ConfigKey = "layer_height_mm", Label = "Layer Height", ConfigType = "dropdown", DefaultValue = "0.016", OptionsJson = """["0.014","0.016","0.028","0.03"]""", Unit = "mm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_MJ_SUPPORT"), ManufacturingProcessId = MjId, ConfigKey = "support_removal", Label = "Support Removal", ConfigType = "dropdown", DefaultValue = "MECHANICAL", OptionsJson = """["MECHANICAL","WATER_JET"]""", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_MJ_FINISH"), ManufacturingProcessId = MjId, ConfigKey = "surface_finish", Label = "Surface Finish Mode", ConfigType = "dropdown", DefaultValue = "MATTE", OptionsJson = """["MATTE","GLOSSY"]""", SortOrder = 30, IsRequired = false, Active = true },

        // ── Binder Jetting ─────────────────────────────────────────────────────
        new() { Id = G("OPT_BJ_INFILTRANT"), ManufacturingProcessId = BjId, ConfigKey = "infiltrant", Label = "Metal Infiltrant", ConfigType = "dropdown", DefaultValue = "NONE", OptionsJson = """["NONE","BRONZE","EPOXY"]""", HelpText = "Post-sintering infiltrant to fill residual porosity", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_BJ_HEAT_TREAT"), ManufacturingProcessId = BjId, ConfigKey = "heat_treatment", Label = "Heat Treatment", ConfigType = "dropdown", DefaultValue = "SINTER", OptionsJson = """["SINTER","SINTER_HIP"]""", SortOrder = 20, IsRequired = false, Active = true },

        // ── DMLS 3D Printing ───────────────────────────────────────────────────
        new() { Id = G("OPT_DMLS_LAYER"), ManufacturingProcessId = DmlsId, ConfigKey = "layer_height_mm", Label = "Layer Height", ConfigType = "dropdown", DefaultValue = "0.04", OptionsJson = """["0.02","0.03","0.04","0.06","0.08"]""", Unit = "mm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_DMLS_HEAT_TREAT"), ManufacturingProcessId = DmlsId, ConfigKey = "heat_treatment", Label = "Heat Treatment", ConfigType = "dropdown", DefaultValue = "STRESS_RELIEF", OptionsJson = """["NONE","STRESS_RELIEF","HIP","ANNEAL"]""", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_DMLS_SUPPORT_REMOVAL"), ManufacturingProcessId = DmlsId, ConfigKey = "support_removal", Label = "Support Removal", ConfigType = "dropdown", DefaultValue = "STANDARD", OptionsJson = """["STANDARD","MACHINED_DATUMS"]""", HelpText = "MACHINED_DATUMS mills support contact faces for tight tolerances", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_DMLS_INSPECTION"), ManufacturingProcessId = DmlsId, ConfigKey = "inspection_type", Label = "Inspection Type", ConfigType = "dropdown", DefaultValue = "VISUAL", OptionsJson = """["VISUAL","CMM","X_RAY","FIRST_ARTICLE"]""", SortOrder = 40, IsRequired = false, Active = true },

        // ── Sheet Metal ────────────────────────────────────────────────────────
        new() { Id = G("OPT_SM_THICKNESS"), ManufacturingProcessId = SheetMetalId, ConfigKey = "sheet_thickness_mm", Label = "Sheet Thickness", ConfigType = "dropdown", DefaultValue = "1.5", OptionsJson = """["0.5","0.8","1.0","1.5","2.0","3.0"]""", Unit = "mm", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_SM_BEND"), ManufacturingProcessId = SheetMetalId, ConfigKey = "bend_radius_factor", Label = "Min Bend Radius", ConfigType = "dropdown", DefaultValue = "1T", OptionsJson = """["0.5T","1T","2T"]""", HelpText = "T = sheet thickness", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_SM_WELD"), ManufacturingProcessId = SheetMetalId, ConfigKey = "welding_required", Label = "Welding Required", ConfigType = "toggle", DefaultValue = "false", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_SM_WELD_TYPE"), ManufacturingProcessId = SheetMetalId, ConfigKey = "welding_type", Label = "Welding Type", ConfigType = "dropdown", DefaultValue = "TIG", OptionsJson = """["TIG","MIG","SPOT"]""", SortOrder = 40, IsRequired = false, Active = true },
        new() { Id = G("OPT_SM_CSINK"), ManufacturingProcessId = SheetMetalId, ConfigKey = "countersink_holes", Label = "Countersink Holes", ConfigType = "toggle", DefaultValue = "false", SortOrder = 50, IsRequired = false, Active = true },
        new() { Id = G("OPT_SM_PEM"), ManufacturingProcessId = SheetMetalId, ConfigKey = "hardware_insertion", Label = "Hardware Insertion (PEM)", ConfigType = "toggle", DefaultValue = "false", SortOrder = 60, IsRequired = false, Active = true },
        new() { Id = G("OPT_SM_PAINT_COLOR"), ManufacturingProcessId = SheetMetalId, ConfigKey = "paint_color", Label = "Paint Color", ConfigType = "text", DefaultValue = null, HelpText = "Specify the desired paint color (e.g. RAL 9005, custom hex)", SortOrder = 70, IsRequired = false, Active = true },

        // ── CNC Anodize Color ──────────────────────────────────────────────────
        new() { Id = G("OPT_CNC_ANODIZE_COLOR"), ManufacturingProcessId = CncId, ConfigKey = "anodize_color", Label = "Anodize Color", ConfigType = "dropdown", DefaultValue = "CLEAR", OptionsJson = """["CLEAR","BLACK","RED","BLUE","GOLD","CUSTOM"]""", HelpText = "Shown when Anodize Clear or Anodize Black finish is selected", SortOrder = 60, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCMILL_ANODIZE_COLOR"), ManufacturingProcessId = CncMillId, ConfigKey = "anodize_color", Label = "Anodize Color", ConfigType = "dropdown", DefaultValue = "CLEAR", OptionsJson = """["CLEAR","BLACK","RED","BLUE","GOLD","CUSTOM"]""", HelpText = "Shown when Anodize Clear or Anodize Black finish is selected", SortOrder = 60, IsRequired = false, Active = true },
        new() { Id = G("OPT_CNCTURN_ANODIZE_COLOR"), ManufacturingProcessId = CncTurnId, ConfigKey = "anodize_color", Label = "Anodize Color", ConfigType = "dropdown", DefaultValue = "CLEAR", OptionsJson = """["CLEAR","BLACK","RED","BLUE","GOLD","CUSTOM"]""", HelpText = "Shown when Anodize Clear or Anodize Black finish is selected", SortOrder = 60, IsRequired = false, Active = true },

        // ── Paint Color (processes that support SF_PAINTED) ────────────────────
        new() { Id = G("OPT_FDM_PAINT_COLOR"), ManufacturingProcessId = FdmId, ConfigKey = "paint_color", Label = "Paint Color", ConfigType = "text", DefaultValue = null, HelpText = "Specify the desired paint color (e.g. RAL 9005, custom hex)", SortOrder = 70, IsRequired = false, Active = true },
        new() { Id = G("OPT_SLA_PAINT_COLOR"), ManufacturingProcessId = SlaDlpId, ConfigKey = "paint_color", Label = "Paint Color", ConfigType = "text", DefaultValue = null, HelpText = "Specify the desired paint color (e.g. RAL 9005, custom hex)", SortOrder = 50, IsRequired = false, Active = true },
        new() { Id = G("OPT_SLS_PAINT_COLOR"), ManufacturingProcessId = SlsId, ConfigKey = "paint_color", Label = "Paint Color", ConfigType = "text", DefaultValue = null, HelpText = "Specify the desired paint color (e.g. RAL 9005, custom hex)", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_MJF_PAINT_COLOR"), ManufacturingProcessId = MjfId, ConfigKey = "paint_color", Label = "Paint Color", ConfigType = "text", DefaultValue = null, HelpText = "Specify the desired paint color (e.g. RAL 9005, custom hex)", SortOrder = 30, IsRequired = false, Active = true },

        // ── Injection Molding ──────────────────────────────────────────────────
        new() { Id = G("OPT_IM_DRAFT"), ManufacturingProcessId = InjectionMoldId, ConfigKey = "draft_angle_deg", Label = "Minimum Draft Angle", ConfigType = "dropdown", DefaultValue = "1.5", OptionsJson = """["0.5","1.0","1.5","2.0","3.0","5.0"]""", Unit = "°", HelpText = "Minimum taper on vertical walls to release part from mold. Steeper = easier ejection.", SortOrder = 10, IsRequired = false, Active = true },
        new() { Id = G("OPT_IM_WALL"), ManufacturingProcessId = InjectionMoldId, ConfigKey = "wall_thickness_mm", Label = "Nominal Wall Thickness", ConfigType = "dropdown", DefaultValue = "2.0", OptionsJson = """["0.5","1.0","1.5","2.0","2.5","3.0","4.0"]""", Unit = "mm", HelpText = "Target wall thickness. Uniform walls reduce warp and sink marks.", SortOrder = 20, IsRequired = false, Active = true },
        new() { Id = G("OPT_IM_GATE"), ManufacturingProcessId = InjectionMoldId, ConfigKey = "gate_type", Label = "Gate Type", ConfigType = "dropdown", DefaultValue = "EDGE", OptionsJson = """["EDGE","PIN","SUBMARINE","HOT_TIP"]""", HelpText = "Gate = where plastic enters the mold cavity.", SortOrder = 30, IsRequired = false, Active = true },
        new() { Id = G("OPT_IM_TEXTURE"), ManufacturingProcessId = InjectionMoldId, ConfigKey = "surface_texture", Label = "Mold Surface Texture", ConfigType = "dropdown", DefaultValue = "SPI_D1", OptionsJson = """["SPI_A1","SPI_A2","SPI_B1","SPI_C1","SPI_D1","SPI_D2","SPI_D3"]""", HelpText = "SPI finish standard: A = mirror, B = semi-gloss, C = matte, D = textured.", SortOrder = 40, IsRequired = false, Active = true },
        new() { Id = G("OPT_IM_QUANTITY"), ManufacturingProcessId = InjectionMoldId, ConfigKey = "annual_volume", Label = "Annual Volume Estimate", ConfigType = "dropdown", DefaultValue = "10000", OptionsJson = """["500","1000","5000","10000","50000","100000","500000"]""", HelpText = "Estimated annual production volume — affects mold steel grade and cavity count recommendation.", SortOrder = 50, IsRequired = false, Active = true },
    ];

    // ── M2M: Process ↔ Material ────────────────────────────────────────────────
    // Returns (processId, materialCode) pairs for seeding the join table
    public static IEnumerable<(Guid ProcessId, Guid MaterialId)> GetProcessMaterialLinks() =>
    [
        // CNC (legacy)
        (CncId, G("MAT_AL6061")), (CncId, G("MAT_AL7075")), (CncId, G("MAT_SS304")),
        (CncId, G("MAT_SS316L")), (CncId, G("MAT_BRASS_C360")), (CncId, G("MAT_COPPER_C110")),
        (CncId, G("MAT_TI6AL4V")), (CncId, G("MAT_PEEK")), (CncId, G("MAT_DELRIN")),
        (CncId, G("MAT_ACRYLIC_CLEAR")),
        // CNC Milling
        (CncMillId, G("MAT_AL6061")), (CncMillId, G("MAT_AL7075")), (CncMillId, G("MAT_SS304")),
        (CncMillId, G("MAT_SS316L")), (CncMillId, G("MAT_BRASS_C360")), (CncMillId, G("MAT_COPPER_C110")),
        (CncMillId, G("MAT_TI6AL4V")), (CncMillId, G("MAT_PEEK")), (CncMillId, G("MAT_DELRIN")),
        (CncMillId, G("MAT_ACRYLIC_CLEAR")),
        // CNC Turning
        (CncTurnId, G("MAT_AL6061")), (CncTurnId, G("MAT_AL7075")), (CncTurnId, G("MAT_SS304")),
        (CncTurnId, G("MAT_SS316L")), (CncTurnId, G("MAT_BRASS_C360")), (CncTurnId, G("MAT_COPPER_C110")),
        (CncTurnId, G("MAT_TI6AL4V")), (CncTurnId, G("MAT_PEEK")), (CncTurnId, G("MAT_DELRIN")),
        (CncTurnId, G("MAT_ACRYLIC_CLEAR")),
        // FDM
        (FdmId, G("MAT_PLA")), (FdmId, G("MAT_PETG")), (FdmId, G("MAT_PETG_CLEAR")), (FdmId, G("MAT_ABS")),
        (FdmId, G("MAT_PA12")), (FdmId, G("MAT_TPU95A")), (FdmId, G("MAT_ASA")),
        (FdmId, G("MAT_PC")), (FdmId, G("MAT_CF_PETG")),
        // SLA/DLP
        (SlaDlpId, G("MAT_STD_RESIN")), (SlaDlpId, G("MAT_TOUGH_RESIN")),
        (SlaDlpId, G("MAT_CLEAR_RESIN")), (SlaDlpId, G("MAT_FLEX_RESIN")),
        (SlaDlpId, G("MAT_CAST_RESIN")), (SlaDlpId, G("MAT_HT_RESIN")),
        // SLS
        (SlsId, G("MAT_PA12_SLS")), (SlsId, G("MAT_PA11_SLS")), (SlsId, G("MAT_PA12GF_SLS")),
        // MJF
        (MjfId, G("MAT_PA12_MJF")), (MjfId, G("MAT_PA12GB_MJF")),
        // Material Jetting
        (MjId, G("MAT_VEROWHITE")), (MjId, G("MAT_VEROBLACK")), (MjId, G("MAT_TANGOPLUS")),
        // Binder Jetting
        (BjId, G("MAT_SS316L_BJ")), (BjId, G("MAT_BRONZE_BJ")), (BjId, G("MAT_SAND_BJ")),
        // DMLS
        (DmlsId, G("MAT_TI6AL4V_DMLS")), (DmlsId, G("MAT_ALSI10MG")), (DmlsId, G("MAT_IN718")), (DmlsId, G("MAT_174PH")),
        // Sheet Metal
        (SheetMetalId, G("MAT_MILD_STEEL")), (SheetMetalId, G("MAT_SS304_SHEET")),
        (SheetMetalId, G("MAT_AL5052")), (SheetMetalId, G("MAT_COPPER_SHEET")), (SheetMetalId, G("MAT_BRASS_SHEET")),
        // Injection Molding
        (InjectionMoldId, G("MAT_PP_IM")), (InjectionMoldId, G("MAT_ABS_IM")),
        (InjectionMoldId, G("MAT_PCABS_IM")), (InjectionMoldId, G("MAT_PA66_IM")), (InjectionMoldId, G("MAT_TPE_IM")),
    ];

    // ── M2M: Process ↔ SurfaceFinish ──────────────────────────────────────────
    public static IEnumerable<(Guid ProcessId, Guid FinishId)> GetProcessFinishLinks() =>
    [
        // CNC (legacy) gets all CNC finishes
        (CncId, G("SF_AS_MACHINED")), (CncId, G("SF_BEAD_BLASTED")), (CncId, G("SF_ANODIZE_CLEAR")),
        (CncId, G("SF_ANODIZE_BLACK")), (CncId, G("SF_ANODIZE_HARD")), (CncId, G("SF_POWDER_COAT")),
        (CncId, G("SF_ELECTROPOLISH")), (CncId, G("SF_BRUSHED")), (CncId, G("SF_MIRROR_POLISH")),
        // CNC Milling finishes
        (CncMillId, G("SF_AS_MACHINED")), (CncMillId, G("SF_BEAD_BLASTED")), (CncMillId, G("SF_ANODIZE_CLEAR")),
        (CncMillId, G("SF_ANODIZE_BLACK")), (CncMillId, G("SF_ANODIZE_HARD")), (CncMillId, G("SF_POWDER_COAT")),
        (CncMillId, G("SF_ELECTROPOLISH")), (CncMillId, G("SF_BRUSHED")), (CncMillId, G("SF_MIRROR_POLISH")),
        // CNC Turning finishes — POWDER_COAT added (turned parts are routinely powder-coated)
        (CncTurnId, G("SF_AS_MACHINED")), (CncTurnId, G("SF_BEAD_BLASTED")), (CncTurnId, G("SF_ANODIZE_CLEAR")),
        (CncTurnId, G("SF_ANODIZE_BLACK")), (CncTurnId, G("SF_ANODIZE_HARD")), (CncTurnId, G("SF_POWDER_COAT")),
        (CncTurnId, G("SF_ELECTROPOLISH")), (CncTurnId, G("SF_BRUSHED")), (CncTurnId, G("SF_MIRROR_POLISH")),
        // FDM gets 3D printing finishes
        (FdmId, G("SF_AS_PRINTED")), (FdmId, G("SF_SANDED")), (FdmId, G("SF_VAPOR_SMOOTH")),
        (FdmId, G("SF_PAINTED")),
        // SLA/DLP gets 3D printing finishes + dye
        (SlaDlpId, G("SF_AS_PRINTED")), (SlaDlpId, G("SF_SANDED")), (SlaDlpId, G("SF_VAPOR_SMOOTH")),
        (SlaDlpId, G("SF_PAINTED")), (SlaDlpId, G("SF_DYE")),
        // SLS finishes — BEAD_BLASTED is the default; DYE is standard for SLS nylon (PA12 dyeing is industry-standard)
        (SlsId, G("SF_BEAD_BLASTED")), (SlsId, G("SF_DYE")), (SlsId, G("SF_SANDED")), (SlsId, G("SF_VAPOR_SMOOTH")),
        (SlsId, G("SF_PAINTED")),
        // MJF finishes — DYE applies to unfilled PA12 MJF
        (MjfId, G("SF_AS_PRINTED")), (MjfId, G("SF_SANDED")), (MjfId, G("SF_VAPOR_SMOOTH")),
        (MjfId, G("SF_PAINTED")), (MjfId, G("SF_DYE")),
        // Material Jetting finishes — PAINTED added (post-paint on MJ parts is common)
        (MjId, G("SF_AS_PRINTED")), (MjId, G("SF_SANDED")), (MjId, G("SF_PAINTED")),
        // Binder Jetting finishes
        (BjId, G("SF_AS_PRINTED")), (BjId, G("SF_BEAD_BLASTED")), (BjId, G("SF_ELECTROPOLISH")),
        // DMLS finishes
        (DmlsId, G("SF_AS_PRINTED")), (DmlsId, G("SF_BEAD_BLASTED")), (DmlsId, G("SF_ELECTROPOLISH")),
        (DmlsId, G("SF_MIRROR_POLISH")),
        // Sheet Metal finishes
        (SheetMetalId, G("SF_DEBURR")), (SheetMetalId, G("SF_POWDER_COAT_SM")),
        (SheetMetalId, G("SF_ZINC_PLATE")), (SheetMetalId, G("SF_GALVANIZE")),
        // Sheet metal: brushed + painted + anodize for aluminum sheet (AL5052 is commonly anodized)
        (SheetMetalId, G("SF_BRUSHED")), (SheetMetalId, G("SF_PAINTED")),
        (SheetMetalId, G("SF_ANODIZE_CLEAR")), (SheetMetalId, G("SF_ANODIZE_BLACK")),
        // Injection Molding finishes
        (InjectionMoldId, G("SF_AS_MOLDED")), (InjectionMoldId, G("SF_PAINTED")),
    ];

    // ── M2M: Material ↔ SurfaceFinish ─────────────────────────────────────────
    // Rules applied:
    //   Anodize (CLEAR/BLACK/HARD) → Al and Ti only. Never plastic.
    //   Mirror Polish             → metals only. POM/PEEK/all polymers excluded.
    //   Electropolish             → metals (Al, SS, Brass, Cu, BJ SS316L, DMLS metals). Not Ti (uncommon).
    //   Powder Coat (CNC)         → Al and SS only. Not Cu, Brass, Ti, or any polymer.
    //   Brushed                   → metals only.
    //   Vapor Smooth              → ABS/ASA (acetone); SLS/MJF PA12/PA11 (tumble); SLA STD/TOUGH/HT resins.
    //                               NOT PLA, PETG, PA12 FDM, TPU, PC, CF-PETG, FLEX/CAST resin, glass-filled.
    //   Dyed                      → SLA std/tough resins; SLS PA12/PA11; MJF PA12 (not glass-filled).
    //                               NOT metals, FDM polymers, flex/cast resin, glass-filled nylon.
    //   Painted                   → all FDM/SLA/SLS/MJF/MJ thermoplastics. Not TPU, FLEX/CAST resin, sand mold.
    //   As-Molded                 → injection molding only.
    public static IEnumerable<(Guid MaterialId, Guid FinishId)> GetMaterialSurfaceFinishLinks() =>
    [
        // ── CNC — Aluminum 6061 (full CNC metal set; anodize YES; all metal finishes) ──────────
        (G("MAT_AL6061"), G("SF_AS_MACHINED")), (G("MAT_AL6061"), G("SF_BEAD_BLASTED")),
        (G("MAT_AL6061"), G("SF_ANODIZE_CLEAR")), (G("MAT_AL6061"), G("SF_ANODIZE_BLACK")),
        (G("MAT_AL6061"), G("SF_ANODIZE_HARD")), (G("MAT_AL6061"), G("SF_POWDER_COAT")),
        (G("MAT_AL6061"), G("SF_ELECTROPOLISH")), (G("MAT_AL6061"), G("SF_BRUSHED")),
        (G("MAT_AL6061"), G("SF_MIRROR_POLISH")),

        // ── CNC — Aluminum 7075 (same as 6061) ──────────────────────────────────────────────────
        (G("MAT_AL7075"), G("SF_AS_MACHINED")), (G("MAT_AL7075"), G("SF_BEAD_BLASTED")),
        (G("MAT_AL7075"), G("SF_ANODIZE_CLEAR")), (G("MAT_AL7075"), G("SF_ANODIZE_BLACK")),
        (G("MAT_AL7075"), G("SF_ANODIZE_HARD")), (G("MAT_AL7075"), G("SF_POWDER_COAT")),
        (G("MAT_AL7075"), G("SF_ELECTROPOLISH")), (G("MAT_AL7075"), G("SF_BRUSHED")),
        (G("MAT_AL7075"), G("SF_MIRROR_POLISH")),

        // ── CNC — Stainless Steel 304 (no anodize — SS cannot be anodized) ───────────────────────
        (G("MAT_SS304"), G("SF_AS_MACHINED")), (G("MAT_SS304"), G("SF_BEAD_BLASTED")),
        (G("MAT_SS304"), G("SF_POWDER_COAT")), (G("MAT_SS304"), G("SF_ELECTROPOLISH")),
        (G("MAT_SS304"), G("SF_BRUSHED")), (G("MAT_SS304"), G("SF_MIRROR_POLISH")),

        // ── CNC — Stainless Steel 316L (same as 304) ────────────────────────────────────────────
        (G("MAT_SS316L"), G("SF_AS_MACHINED")), (G("MAT_SS316L"), G("SF_BEAD_BLASTED")),
        (G("MAT_SS316L"), G("SF_POWDER_COAT")), (G("MAT_SS316L"), G("SF_ELECTROPOLISH")),
        (G("MAT_SS316L"), G("SF_BRUSHED")), (G("MAT_SS316L"), G("SF_MIRROR_POLISH")),

        // ── CNC — Brass C360 (Cu-Zn alloy; no anodize, no powder coat for CNC) ───────────────────
        (G("MAT_BRASS_C360"), G("SF_AS_MACHINED")), (G("MAT_BRASS_C360"), G("SF_BEAD_BLASTED")),
        (G("MAT_BRASS_C360"), G("SF_ELECTROPOLISH")), (G("MAT_BRASS_C360"), G("SF_BRUSHED")),
        (G("MAT_BRASS_C360"), G("SF_MIRROR_POLISH")),

        // ── CNC — Copper C110 (no anodize) ──────────────────────────────────────────────────────
        (G("MAT_COPPER_C110"), G("SF_AS_MACHINED")), (G("MAT_COPPER_C110"), G("SF_BEAD_BLASTED")),
        (G("MAT_COPPER_C110"), G("SF_ELECTROPOLISH")), (G("MAT_COPPER_C110"), G("SF_BRUSHED")),
        (G("MAT_COPPER_C110"), G("SF_MIRROR_POLISH")),

        // ── CNC — Titanium Ti-6Al-4V (anodize YES; no electropolish standard for CNC Ti) ─────────
        (G("MAT_TI6AL4V"), G("SF_AS_MACHINED")), (G("MAT_TI6AL4V"), G("SF_BEAD_BLASTED")),
        (G("MAT_TI6AL4V"), G("SF_ANODIZE_CLEAR")), (G("MAT_TI6AL4V"), G("SF_ANODIZE_BLACK")),
        (G("MAT_TI6AL4V"), G("SF_ANODIZE_HARD")), (G("MAT_TI6AL4V"), G("SF_BRUSHED")),
        (G("MAT_TI6AL4V"), G("SF_MIRROR_POLISH")),

        // ── CNC — PEEK (engineering polymer; only mechanical finishes; no anodize/mirror/electropolish) ─
        (G("MAT_PEEK"), G("SF_AS_MACHINED")), (G("MAT_PEEK"), G("SF_BEAD_BLASTED")),

        // ── CNC — Delrin/POM (semi-crystalline polymer; no anodize, no mirror polish, no electropolish) ─
        // Mirror polishing POM is physically impossible — it's semi-crystalline and cannot achieve
        // specular metal-grade surface. Anodizing requires electrochemical oxidation of metal.
        (G("MAT_DELRIN"), G("SF_AS_MACHINED")), (G("MAT_DELRIN"), G("SF_BEAD_BLASTED")),

        // ── CNC — Clear Acrylic/PMMA (transparent polymer; mechanical finishes only) ─────────────
        (G("MAT_ACRYLIC_CLEAR"), G("SF_AS_MACHINED")), (G("MAT_ACRYLIC_CLEAR"), G("SF_BEAD_BLASTED")),

        // ── FDM — PLA (PLA not acetone-soluble; no vapor smooth) ─────────────────────────────────
        (G("MAT_PLA"), G("SF_AS_PRINTED")), (G("MAT_PLA"), G("SF_SANDED")), (G("MAT_PLA"), G("SF_PAINTED")),

        // ── FDM — PETG (no vapor smooth) ─────────────────────────────────────────────────────────
        (G("MAT_PETG"), G("SF_AS_PRINTED")), (G("MAT_PETG"), G("SF_SANDED")), (G("MAT_PETG"), G("SF_PAINTED")),
        (G("MAT_PETG_CLEAR"), G("SF_AS_PRINTED")), (G("MAT_PETG_CLEAR"), G("SF_SANDED")),
        (G("MAT_PETG_CLEAR"), G("SF_PAINTED")),

        // ── FDM — ABS (acetone vapor smoothing supported) ────────────────────────────────────────
        (G("MAT_ABS"), G("SF_AS_PRINTED")), (G("MAT_ABS"), G("SF_SANDED")),
        (G("MAT_ABS"), G("SF_VAPOR_SMOOTH")), (G("MAT_ABS"), G("SF_PAINTED")),

        // ── FDM — PA12 Nylon FDM (no vapor smooth for FDM nylon) ─────────────────────────────────
        (G("MAT_PA12"), G("SF_AS_PRINTED")), (G("MAT_PA12"), G("SF_SANDED")), (G("MAT_PA12"), G("SF_PAINTED")),

        // ── FDM — TPU 95A (flexible elastomer; sanding tears surface; only as-printed) ─────────────
        (G("MAT_TPU95A"), G("SF_AS_PRINTED")),

        // ── FDM — ASA (UV-stable ABS-like; acetone vapor smooth supported) ──────────────────────
        (G("MAT_ASA"), G("SF_AS_PRINTED")), (G("MAT_ASA"), G("SF_SANDED")),
        (G("MAT_ASA"), G("SF_VAPOR_SMOOTH")), (G("MAT_ASA"), G("SF_PAINTED")),

        // ── FDM — Polycarbonate (no vapor smooth) ─────────────────────────────────────────────────
        (G("MAT_PC"), G("SF_AS_PRINTED")), (G("MAT_PC"), G("SF_SANDED")), (G("MAT_PC"), G("SF_PAINTED")),

        // ── FDM — CF-PETG (carbon fiber reinforced; sanding reveals fibers) ──────────────────────
        (G("MAT_CF_PETG"), G("SF_AS_PRINTED")), (G("MAT_CF_PETG"), G("SF_SANDED")), (G("MAT_CF_PETG"), G("SF_PAINTED")),

        // ── SLA/DLP — Standard Resin (dyeable; vapor smooth via IPA treatment) ──────────────────
        (G("MAT_STD_RESIN"), G("SF_AS_PRINTED")), (G("MAT_STD_RESIN"), G("SF_SANDED")),
        (G("MAT_STD_RESIN"), G("SF_VAPOR_SMOOTH")), (G("MAT_STD_RESIN"), G("SF_PAINTED")),
        (G("MAT_STD_RESIN"), G("SF_DYE")),

        // ── SLA/DLP — Clear Resin (transparent photopolymer; avoid dye by default) ───────────────
        (G("MAT_CLEAR_RESIN"), G("SF_AS_PRINTED")), (G("MAT_CLEAR_RESIN"), G("SF_SANDED")),
        (G("MAT_CLEAR_RESIN"), G("SF_VAPOR_SMOOTH")),

        // ── SLA/DLP — Tough Resin ─────────────────────────────────────────────────────────────────
        (G("MAT_TOUGH_RESIN"), G("SF_AS_PRINTED")), (G("MAT_TOUGH_RESIN"), G("SF_SANDED")),
        (G("MAT_TOUGH_RESIN"), G("SF_VAPOR_SMOOTH")), (G("MAT_TOUGH_RESIN"), G("SF_PAINTED")),
        (G("MAT_TOUGH_RESIN"), G("SF_DYE")),

        // ── SLA/DLP — Flexible Resin (rubber-like; no vapor smooth, no reliable paint, no dye) ───
        (G("MAT_FLEX_RESIN"), G("SF_AS_PRINTED")), (G("MAT_FLEX_RESIN"), G("SF_SANDED")),

        // ── SLA/DLP — Castable Resin (investment casting; minimal post-process — it gets burned out) ─
        (G("MAT_CAST_RESIN"), G("SF_AS_PRINTED")), (G("MAT_CAST_RESIN"), G("SF_SANDED")),

        // ── SLA/DLP — High-Temp Resin ─────────────────────────────────────────────────────────────
        (G("MAT_HT_RESIN"), G("SF_AS_PRINTED")), (G("MAT_HT_RESIN"), G("SF_SANDED")),
        (G("MAT_HT_RESIN"), G("SF_VAPOR_SMOOTH")), (G("MAT_HT_RESIN"), G("SF_PAINTED")),

        // ── SLS — PA12 (bead blast is the default SLS finish; dyeing is industry-standard) ─────────
        (G("MAT_PA12_SLS"), G("SF_BEAD_BLASTED")), (G("MAT_PA12_SLS"), G("SF_DYE")),
        (G("MAT_PA12_SLS"), G("SF_SANDED")), (G("MAT_PA12_SLS"), G("SF_VAPOR_SMOOTH")),
        (G("MAT_PA12_SLS"), G("SF_PAINTED")),

        // ── SLS — PA11 (bio-based; same post-process capability as PA12 SLS) ──────────────────────
        (G("MAT_PA11_SLS"), G("SF_BEAD_BLASTED")), (G("MAT_PA11_SLS"), G("SF_DYE")),
        (G("MAT_PA11_SLS"), G("SF_SANDED")), (G("MAT_PA11_SLS"), G("SF_VAPOR_SMOOTH")),
        (G("MAT_PA11_SLS"), G("SF_PAINTED")),

        // ── SLS — PA12GF (glass-filled; bead blast supported; no dye — glass disrupts dye uptake) ─
        (G("MAT_PA12GF_SLS"), G("SF_BEAD_BLASTED")), (G("MAT_PA12GF_SLS"), G("SF_SANDED")),
        (G("MAT_PA12GF_SLS"), G("SF_PAINTED")),

        // ── MJF — PA12 (dyeable; tumble/vapor smooth supported) ────────────────────────────────────
        (G("MAT_PA12_MJF"), G("SF_AS_PRINTED")), (G("MAT_PA12_MJF"), G("SF_SANDED")),
        (G("MAT_PA12_MJF"), G("SF_VAPOR_SMOOTH")), (G("MAT_PA12_MJF"), G("SF_PAINTED")),
        (G("MAT_PA12_MJF"), G("SF_DYE")),

        // ── MJF — PA12 Glass Bead (no vapor smooth, no dye — glass fill disrupts both) ────────────
        (G("MAT_PA12GB_MJF"), G("SF_AS_PRINTED")), (G("MAT_PA12GB_MJF"), G("SF_SANDED")),
        (G("MAT_PA12GB_MJF"), G("SF_PAINTED")),

        // ── Material Jetting — VeroWhite (smooth as-printed; painted for colour/texture) ──────────
        (G("MAT_VEROWHITE"), G("SF_AS_PRINTED")), (G("MAT_VEROWHITE"), G("SF_SANDED")),
        (G("MAT_VEROWHITE"), G("SF_PAINTED")),

        // ── Material Jetting — VeroBlack ────────────────────────────────────────────────────────────
        (G("MAT_VEROBLACK"), G("SF_AS_PRINTED")), (G("MAT_VEROBLACK"), G("SF_SANDED")),
        (G("MAT_VEROBLACK"), G("SF_PAINTED")),

        // ── Material Jetting — TangoPlus (rubber-like; painting not practical; only as-printed) ───
        (G("MAT_TANGOPLUS"), G("SF_AS_PRINTED")),

        // ── Binder Jetting — SS316L (sintered metal; bead blast and electropolish standard) ───────
        (G("MAT_SS316L_BJ"), G("SF_AS_PRINTED")), (G("MAT_SS316L_BJ"), G("SF_BEAD_BLASTED")),
        (G("MAT_SS316L_BJ"), G("SF_ELECTROPOLISH")),

        // ── Binder Jetting — Bronze (sintered; limited to bead blast) ──────────────────────────────
        (G("MAT_BRONZE_BJ"), G("SF_AS_PRINTED")), (G("MAT_BRONZE_BJ"), G("SF_BEAD_BLASTED")),

        // ── Binder Jetting — Silica Sand (casting mold; no post-process finishes) ─────────────────
        (G("MAT_SAND_BJ"), G("SF_AS_PRINTED")),

        // ── DMLS — Ti-6Al-4V (hard alloy; no mirror polish standard for DMLS Ti) ──────────────────
        (G("MAT_TI6AL4V_DMLS"), G("SF_AS_PRINTED")), (G("MAT_TI6AL4V_DMLS"), G("SF_BEAD_BLASTED")),
        (G("MAT_TI6AL4V_DMLS"), G("SF_ELECTROPOLISH")),

        // ── DMLS — AlSi10Mg (aluminum alloy; anodize IS done in practice; mirror polish achievable) ──
        (G("MAT_ALSI10MG"), G("SF_AS_PRINTED")), (G("MAT_ALSI10MG"), G("SF_BEAD_BLASTED")),
        (G("MAT_ALSI10MG"), G("SF_ELECTROPOLISH")), (G("MAT_ALSI10MG"), G("SF_MIRROR_POLISH")),
        (G("MAT_ALSI10MG"), G("SF_ANODIZE_CLEAR")), (G("MAT_ALSI10MG"), G("SF_ANODIZE_BLACK")),

        // ── DMLS — Inconel 718 (aerospace superalloy; mirror polish done in practice) ────────────
        (G("MAT_IN718"), G("SF_AS_PRINTED")), (G("MAT_IN718"), G("SF_BEAD_BLASTED")),
        (G("MAT_IN718"), G("SF_ELECTROPOLISH")), (G("MAT_IN718"), G("SF_MIRROR_POLISH")),

        // ── DMLS — 17-4PH Stainless (precipitation-hardened SS; mirror polish achievable) ─────────
        (G("MAT_174PH"), G("SF_AS_PRINTED")), (G("MAT_174PH"), G("SF_BEAD_BLASTED")),
        (G("MAT_174PH"), G("SF_ELECTROPOLISH")), (G("MAT_174PH"), G("SF_MIRROR_POLISH")),

        // ── Sheet Metal — Mild Steel (full SM finish set; zinc and galvanize for mild steel only) ──
        (G("MAT_MILD_STEEL"), G("SF_DEBURR")), (G("MAT_MILD_STEEL"), G("SF_POWDER_COAT_SM")),
        (G("MAT_MILD_STEEL"), G("SF_ZINC_PLATE")), (G("MAT_MILD_STEEL"), G("SF_GALVANIZE")),
        (G("MAT_MILD_STEEL"), G("SF_PAINTED")), (G("MAT_MILD_STEEL"), G("SF_BRUSHED")),

        // ── Sheet Metal — SS304 Sheet (no zinc plate, no galvanize — SS doesn't need corrosion protection) ─
        (G("MAT_SS304_SHEET"), G("SF_DEBURR")), (G("MAT_SS304_SHEET"), G("SF_POWDER_COAT_SM")),
        (G("MAT_SS304_SHEET"), G("SF_BRUSHED")), (G("MAT_SS304_SHEET"), G("SF_PAINTED")),

        // ── Sheet Metal — Aluminum 5052 Sheet (anodizable — very common for enclosures and panels) ──
        (G("MAT_AL5052"), G("SF_DEBURR")), (G("MAT_AL5052"), G("SF_POWDER_COAT_SM")),
        (G("MAT_AL5052"), G("SF_BRUSHED")), (G("MAT_AL5052"), G("SF_PAINTED")),
        (G("MAT_AL5052"), G("SF_ANODIZE_CLEAR")), (G("MAT_AL5052"), G("SF_ANODIZE_BLACK")),

        // ── Sheet Metal — Copper Sheet (limited; no powder coat, no zinc/galvanize) ──────────────
        (G("MAT_COPPER_SHEET"), G("SF_DEBURR")), (G("MAT_COPPER_SHEET"), G("SF_BRUSHED")),
        (G("MAT_COPPER_SHEET"), G("SF_PAINTED")),

        // ── Sheet Metal — Brass Sheet ─────────────────────────────────────────────────────────────
        (G("MAT_BRASS_SHEET"), G("SF_DEBURR")), (G("MAT_BRASS_SHEET"), G("SF_BRUSHED")),
        (G("MAT_BRASS_SHEET"), G("SF_PAINTED")),

        // ── Injection Molding — PP (as-molded standard; painted for aesthetic parts) ────────────
        (G("MAT_PP_IM"), G("SF_AS_MOLDED")), (G("MAT_PP_IM"), G("SF_PAINTED")),

        // ── Injection Molding — ABS (as-molded; painting and primer common) ──────────────────────
        (G("MAT_ABS_IM"), G("SF_AS_MOLDED")), (G("MAT_ABS_IM"), G("SF_PAINTED")),

        // ── Injection Molding — PC/ABS Blend ─────────────────────────────────────────────────────
        (G("MAT_PCABS_IM"), G("SF_AS_MOLDED")), (G("MAT_PCABS_IM"), G("SF_PAINTED")),

        // ── Injection Molding — PA66 (nylon; painting with adhesion primer) ──────────────────────
        (G("MAT_PA66_IM"), G("SF_AS_MOLDED")), (G("MAT_PA66_IM"), G("SF_PAINTED")),

        // ── Injection Molding — TPE Overmold (rubber-like; painting not practical) ────────────────
        (G("MAT_TPE_IM"), G("SF_AS_MOLDED")),
    ];

    // ── M2M: Process ↔ ToleranceClass ─────────────────────────────────────────
    public static IEnumerable<(Guid ProcessId, Guid ToleranceId)> GetProcessToleranceLinks() =>
    [
        // CNC (legacy) gets ISO 2768 + IT grades
        (CncId, G("TOL_ISO2768_F")), (CncId, G("TOL_ISO2768_M")), (CncId, G("TOL_ISO2768_C")),
        (CncId, G("TOL_IT6")), (CncId, G("TOL_IT7")), (CncId, G("TOL_IT8")),
        // CNC Milling gets ISO 2768 + IT grades
        (CncMillId, G("TOL_ISO2768_F")), (CncMillId, G("TOL_ISO2768_M")), (CncMillId, G("TOL_ISO2768_C")),
        (CncMillId, G("TOL_IT6")), (CncMillId, G("TOL_IT7")), (CncMillId, G("TOL_IT8")),
        // CNC Turning gets ISO 2768 + IT grades
        (CncTurnId, G("TOL_ISO2768_F")), (CncTurnId, G("TOL_ISO2768_M")), (CncTurnId, G("TOL_ISO2768_C")),
        (CncTurnId, G("TOL_IT6")), (CncTurnId, G("TOL_IT7")), (CncTurnId, G("TOL_IT8")),
        // FDM gets ISO 2768 coarse/very coarse + FDM-specific
        (FdmId, G("TOL_ISO2768_C")), (FdmId, G("TOL_ISO2768_V")),
        (FdmId, G("TOL_FDM_STD")), (FdmId, G("TOL_FDM_FINE")),
        // SLA/DLP gets ISO 2768 fine/medium + SLA-specific
        (SlaDlpId, G("TOL_ISO2768_F")), (SlaDlpId, G("TOL_ISO2768_M")),
        (SlaDlpId, G("TOL_SLA_STD")), (SlaDlpId, G("TOL_SLA_FINE")),
        // SLS gets ISO 2768 coarse + SLS-specific
        (SlsId, G("TOL_ISO2768_C")), (SlsId, G("TOL_ISO2768_V")), (SlsId, G("TOL_SLS_STD")),
        // MJF gets ISO 2768 coarse + MJF-specific
        (MjfId, G("TOL_ISO2768_C")), (MjfId, G("TOL_ISO2768_V")), (MjfId, G("TOL_MJF_STD")),
        // Material Jetting gets ISO 2768 fine/medium + MJ-specific
        (MjId, G("TOL_ISO2768_F")), (MjId, G("TOL_ISO2768_M")), (MjId, G("TOL_MJ_STD")),
        // Binder Jetting gets ISO 2768 medium/coarse + BJ-specific
        (BjId, G("TOL_ISO2768_M")), (BjId, G("TOL_ISO2768_C")), (BjId, G("TOL_BJ_STD")),
        // DMLS gets ISO 2768 fine/medium + DMLS-specific
        (DmlsId, G("TOL_ISO2768_F")), (DmlsId, G("TOL_ISO2768_M")), (DmlsId, G("TOL_DMLS_STD")), (DmlsId, G("TOL_DMLS_FINE")),
        // Sheet Metal gets ISO 2768
        (SheetMetalId, G("TOL_ISO2768_M")), (SheetMetalId, G("TOL_ISO2768_C")), (SheetMetalId, G("TOL_ISO2768_V")),
        // Injection Molding gets SPI-based tolerances
        (InjectionMoldId, G("TOL_ISO2768_C")), (InjectionMoldId, G("TOL_ISO2768_V")),
        (InjectionMoldId, G("TOL_IM_STD")), (InjectionMoldId, G("TOL_IM_FINE")),
    ];
}
