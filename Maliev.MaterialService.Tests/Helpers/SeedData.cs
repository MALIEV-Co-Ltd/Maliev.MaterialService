using Maliev.MaterialService.Domain.Entities;
using Maliev.MaterialService.Infrastructure.Persistence;
using System.Collections.Generic;

namespace Maliev.MaterialService.Tests.Helpers;

public static class SeedData
{
    public static void Initialize(MaterialDbContext context)
    {
        // Seed test data for materials with relationships
        if (!context.Materials.Any() && !context.ManufacturingProcesses.Any())
        {
            // Create colors
            var blueColor = new Color { Name = "Blue", HexCode = "#0000FF" };
            var silverColor = new Color { Name = "Silver", HexCode = "#C0C0C0" };
            context.Colors.AddRange(blueColor, silverColor);

            // Create manufacturing processes
            var cncMachining = new ManufacturingProcess { Name = "CNC Machining", Code = "CNC-MACHINING" };
            var injectionMolding = new ManufacturingProcess { Name = "Injection Molding", Code = "INJECTION-MOLDING" };
            context.ManufacturingProcesses.AddRange(cncMachining, injectionMolding);

            // Create mechanical properties
            var tensileStrength = new MechanicalProperty { Name = "Tensile Strength", Unit = "MPa" };
            context.MechanicalProperties.Add(tensileStrength);

            context.SaveChanges();

            // Create materials
            var polycarbonate = new Material
            {
                Name = "Polycarbonate",
                Code = "PC-001",
                Description = "Clear polycarbonate sheet",
                PricePerUnit = 25.00m,
                StockLevel = 100,
                AvailableColors = new List<Color> { blueColor },
                ManufacturingProcesses = new List<ManufacturingProcess> { injectionMolding }
            };

            var aluminum6061 = new Material
            {
                Name = "Aluminum 6061",
                Code = "AL-6061",
                Description = "Aluminum alloy 6061",
                PricePerUnit = 15.00m,
                StockLevel = 200,
                AvailableColors = new List<Color> { silverColor },
                ManufacturingProcesses = new List<ManufacturingProcess> { cncMachining }
            };

            var absPlastic = new Material
            {
                Name = "ABS Plastic",
                Code = "ABS-001",
                Description = "ABS plastic material",
                PricePerUnit = 10.00m,
                StockLevel = 150,
                AvailableColors = new List<Color> { blueColor },
                ManufacturingProcesses = new List<ManufacturingProcess> { injectionMolding }
            };

            context.Materials.AddRange(polycarbonate, aluminum6061, absPlastic);
            context.SaveChanges();

            // Add mechanical property for polycarbonate (tensile strength between 50-150 MPa)
            context.MaterialMechanicalProperties.Add(new MaterialMechanicalProperty
            {
                MaterialId = polycarbonate.Id,
                MechanicalPropertyId = tensileStrength.Id,
                Value = 100m // Within 50-150 range
            });

            context.SaveChanges();
        }
    }
}
