using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Data;
using Maliev.MaterialService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maliev.MaterialService.Api.Services
{
    public class MaterialServiceService : IMaterialServiceService
    {
        private readonly MaterialContext _context;
        private readonly ILogger<MaterialServiceService> _logger;

        public MaterialServiceService(MaterialContext context, ILogger<MaterialServiceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Color operations
        public async Task<IEnumerable<ColorDto>> GetAllColorsAsync()
        {
            var colors = await _context.Color.ToListAsync();
            return colors.Select(c => new ColorDto { Id = c.Id, Name = c.Name });
        }

        public async Task<ColorDto?> GetColorByIdAsync(int id)
        {
            var color = await _context.Color.FindAsync(id);
            return color == null ? null : new ColorDto { Id = color.Id, Name = color.Name };
        }

        public async Task<ColorDto> CreateColorAsync(CreateColorRequest request)
        {
            var color = new Color { Name = request.Name };
            _context.Color.Add(color);
            await _context.SaveChangesAsync();
            return new ColorDto { Id = color.Id, Name = color.Name };
        }

        public async Task<ColorDto?> UpdateColorAsync(UpdateColorRequest request)
        {
            var color = await _context.Color.FindAsync(request.Id);
            if (color == null)
            {
                return null;
            }
            color.Name = request.Name;
            await _context.SaveChangesAsync();
            return new ColorDto { Id = color.Id, Name = color.Name };
        }

        public async Task<bool> DeleteColorAsync(int id)
        {
            var color = await _context.Color.FindAsync(id);
            if (color == null)
            {
                return false;
            }
            _context.Color.Remove(color);
            await _context.SaveChangesAsync();
            return true;
        }

        // MaterialGroup operations
        public async Task<IEnumerable<MaterialGroupDto>> GetAllMaterialGroupsAsync()
        {
            var materialGroups = await _context.MaterialGroup.ToListAsync();
            return materialGroups.Select(mg => new MaterialGroupDto { Id = mg.Id, Name = mg.Name, Description = mg.Description });
        }

        public async Task<MaterialGroupDto?> GetMaterialGroupByIdAsync(int id)
        {
            var materialGroup = await _context.MaterialGroup.FindAsync(id);
            return materialGroup == null ? null : new MaterialGroupDto { Id = materialGroup.Id, Name = materialGroup.Name, Description = materialGroup.Description };
        }

        public async Task<MaterialGroupDto> CreateMaterialGroupAsync(CreateMaterialGroupRequest request)
        {
            var materialGroup = new MaterialGroup { Name = request.Name, Description = request.Description };
            _context.MaterialGroup.Add(materialGroup);
            await _context.SaveChangesAsync();
            return new MaterialGroupDto { Id = materialGroup.Id, Name = materialGroup.Name, Description = materialGroup.Description };
        }

        public async Task<MaterialGroupDto?> UpdateMaterialGroupAsync(UpdateMaterialGroupRequest request)
        {
            var materialGroup = await _context.MaterialGroup.FindAsync(request.Id);
            if (materialGroup == null)
            {
                return null;
            }
            materialGroup.Name = request.Name;
            materialGroup.Description = request.Description;
            await _context.SaveChangesAsync();
            return new MaterialGroupDto { Id = materialGroup.Id, Name = materialGroup.Name, Description = materialGroup.Description };
        }

        public async Task<bool> DeleteMaterialGroupAsync(int id)
        {
            var materialGroup = await _context.MaterialGroup.FindAsync(id);
            if (materialGroup == null)
            {
                return false;
            }
            _context.MaterialGroup.Remove(materialGroup);
            await _context.SaveChangesAsync();
            return true;
        }

        // Material operations
        public async Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync()
        {
            var materials = await _context.Material.ToListAsync();
            return materials.Select(m => new MaterialDto
            {
                Id = m.Id,
                MaterialGroupId = m.MaterialGroupId,
                Machinable = m.Machinable,
                Printable = m.Printable,
                Name = m.Name,
                Aisi = m.Aisi,
                Din = m.Din,
                Bts = m.Bts,
                Jis = m.Jis,
                Uns = m.Uns,
                En = m.En,
                Afnor = m.Afnor,
                Uni = m.Uni,
                Sis = m.Sis,
                Sae = m.Sae,
                Astm = m.Astm,
                Ams = m.Ams,
                MaterialNumber = m.MaterialNumber,
                ManufacturerReference = m.ManufacturerReference,
                HardnessBrinell = m.HardnessBrinell,
                HardnessKnoop = m.HardnessKnoop,
                HardnessRockwellA = m.HardnessRockwellA,
                HardnessRockwellB = m.HardnessRockwellB,
                HardnessRockwellC = m.HardnessRockwellC,
                HardnessVickers = m.HardnessVickers,
                DensityKilogramPerCubicMeter = m.DensityKilogramPerCubicMeter,
                TensileStrengthUltimateGigaPascal = m.TensileStrengthUltimateGigaPascal,
                TensileStrengthYieldMegaPascal = m.TensileStrengthYieldMegaPascal,
                MachinabilityPercent = m.MachinabilityPercent,
                ShearModulusGigaPascal = m.ShearModulusGigaPascal,
                ThermalConductivityWattPerMeterKelvin = m.ThermalConductivityWattPerMeterKelvin,
                Url = m.Url,
                PricePerKilogram = m.PricePerKilogram,
                CurrencyId = m.CurrencyId,
                Comment = m.Comment
            });
        }

        public async Task<MaterialDto?> GetMaterialByIdAsync(int id)
        {
            var material = await _context.Material.FindAsync(id);
            return material == null ? null : new MaterialDto
            {
                Id = material.Id,
                MaterialGroupId = material.MaterialGroupId,
                Machinable = material.Machinable,
                Printable = material.Printable,
                Name = material.Name,
                Aisi = material.Aisi,
                Din = material.Din,
                Bts = material.Bts,
                Jis = material.Jis,
                Uns = material.Uns,
                En = material.En,
                Afnor = material.Afnor,
                Uni = material.Uni,
                Sis = material.Sis,
                Sae = material.Sae,
                Astm = material.Astm,
                Ams = material.Ams,
                MaterialNumber = material.MaterialNumber,
                ManufacturerReference = material.ManufacturerReference,
                HardnessBrinell = material.HardnessBrinell,
                HardnessKnoop = material.HardnessKnoop,
                HardnessRockwellA = material.HardnessRockwellA,
                HardnessRockwellB = material.HardnessRockwellB,
                HardnessRockwellC = material.HardnessRockwellC,
                HardnessVickers = material.HardnessVickers,
                DensityKilogramPerCubicMeter = material.DensityKilogramPerCubicMeter,
                TensileStrengthUltimateGigaPascal = material.TensileStrengthUltimateGigaPascal,
                TensileStrengthYieldMegaPascal = material.TensileStrengthYieldMegaPascal,
                MachinabilityPercent = material.MachinabilityPercent,
                ShearModulusGigaPascal = material.ShearModulusGigaPascal,
                ThermalConductivityWattPerMeterKelvin = material.ThermalConductivityWattPerMeterKelvin,
                Url = material.Url,
                PricePerKilogram = material.PricePerKilogram,
                CurrencyId = material.CurrencyId,
                Comment = material.Comment
            };
        }

        public async Task<MaterialDto> CreateMaterialAsync(CreateMaterialRequest request)
        {
            var material = new Material
            {
                MaterialGroupId = request.MaterialGroupId,
                Machinable = request.Machinable,
                Printable = request.Printable,
                Name = request.Name,
                Aisi = request.Aisi,
                Din = request.Din,
                Bts = request.Bts,
                Jis = request.Jis,
                Uns = request.Uns,
                En = request.En,
                Afnor = request.Afnor,
                Uni = request.Uni,
                Sis = request.Sis,
                Sae = request.Sae,
                Astm = request.Astm,
                Ams = request.Ams,
                MaterialNumber = request.MaterialNumber,
                ManufacturerReference = request.ManufacturerReference,
                HardnessBrinell = request.HardnessBrinell,
                HardnessKnoop = request.HardnessKnoop,
                HardnessRockwellA = request.HardnessRockwellA,
                HardnessRockwellB = request.HardnessRockwellB,
                HardnessRockwellC = request.HardnessRockwellC,
                HardnessVickers = request.HardnessVickers,
                DensityKilogramPerCubicMeter = request.DensityKilogramPerCubicMeter,
                TensileStrengthUltimateGigaPascal = request.TensileStrengthUltimateGigaPascal,
                TensileStrengthYieldMegaPascal = request.TensileStrengthYieldMegaPascal,
                MachinabilityPercent = request.MachinabilityPercent,
                ShearModulusGigaPascal = request.ShearModulusGigaPascal,
                ThermalConductivityWattPerMeterKelvin = request.ThermalConductivityWattPerMeterKelvin,
                Url = request.Url,
                PricePerKilogram = request.PricePerKilogram,
                CurrencyId = request.CurrencyId,
                Comment = request.Comment
            };
            _context.Material.Add(material);
            await _context.SaveChangesAsync();
            return new MaterialDto
            {
                Id = material.Id,
                MaterialGroupId = material.MaterialGroupId,
                Machinable = material.Machinable,
                Printable = material.Printable,
                Name = material.Name,
                Aisi = material.Aisi,
                Din = material.Din,
                Bts = material.Bts,
                Jis = material.Jis,
                Uns = material.Uns,
                En = material.En,
                Afnor = material.Afnor,
                Uni = material.Uni,
                Sis = material.Sis,
                Sae = material.Sae,
                Astm = material.Astm,
                Ams = material.Ams,
                MaterialNumber = material.MaterialNumber,
                ManufacturerReference = material.ManufacturerReference,
                HardnessBrinell = request.HardnessBrinell,
                HardnessKnoop = request.HardnessKnoop,
                HardnessRockwellA = request.HardnessRockwellA,
                HardnessRockwellB = request.HardnessRockwellB,
                HardnessRockwellC = request.HardnessRockwellC,
                HardnessVickers = request.HardnessVickers,
                DensityKilogramPerCubicMeter = request.DensityKilogramPerCubicMeter,
                TensileStrengthUltimateGigaPascal = request.TensileStrengthUltimateGigaPascal,
                TensileStrengthYieldMegaPascal = request.TensileStrengthYieldMegaPascal,
                MachinabilityPercent = request.MachinabilityPercent,
                ShearModulusGigaPascal = request.ShearModulusGigaPascal,
                ThermalConductivityWattPerMeterKelvin = request.ThermalConductivityWattPerMeterKelvin,
                Url = request.Url,
                PricePerKilogram = request.PricePerKilogram,
                CurrencyId = request.CurrencyId,
                Comment = request.Comment
            };
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _context.Material.FindAsync(id);
            if (material == null)
            {
                return false;
            }
            _context.Material.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }

        // MaterialHasColor operations
        public async Task<IEnumerable<MaterialHasColorDto>> GetAllMaterialHasColorsAsync()
        {
            var materialHasColors = await _context.MaterialHasColor.ToListAsync();
            return materialHasColors.Select(mhc => new MaterialHasColorDto { Id = mhc.Id, MaterialId = mhc.MaterialId, ColorId = mhc.ColorId });
        }

        public async Task<MaterialHasColorDto?> GetMaterialHasColorByIdAsync(int id)
        {
            var materialHasColor = await _context.MaterialHasColor.FindAsync(id);
            return materialHasColor == null ? null : new MaterialHasColorDto { Id = materialHasColor.Id, MaterialId = materialHasColor.MaterialId, ColorId = materialHasColor.ColorId };
        }

        // MaterialHasSupplier operations
        public async Task<IEnumerable<MaterialHasSupplierDto>> GetAllMaterialHasSuppliersAsync()
        {
            var materialHasSuppliers = await _context.MaterialHasSupplier.ToListAsync();
            return materialHasSuppliers.Select(mhs => new MaterialHasSupplierDto { Id = mhs.Id, MaterialId = mhs.MaterialId, SupplierId = mhs.SupplierId });
        }

        public async Task<MaterialHasSupplierDto?> GetMaterialHasSupplierByIdAsync(int id)
        {
            var materialHasSupplier = await _context.MaterialHasSupplier.FindAsync(id);
            return materialHasSupplier == null ? null : new MaterialHasSupplierDto { Id = materialHasSupplier.Id, MaterialId = materialHasSupplier.MaterialId, SupplierId = materialHasSupplier.SupplierId };
        }

        // MaterialHasSurfaceFinish operations
        public async Task<IEnumerable<MaterialHasSurfaceFinishDto>> GetAllMaterialHasSurfaceFinishesAsync()
        {
            var materialHasSurfaceFinishes = await _context.MaterialHasSurfaceFinish.ToListAsync();
            return materialHasSurfaceFinishes.Select(mhf => new MaterialHasSurfaceFinishDto { Id = mhf.Id, MaterialId = mhf.MaterialId, SurfaceFinishId = mhf.SurfaceFinishId });
        }

        public async Task<MaterialHasSurfaceFinishDto?> GetMaterialHasSurfaceFinishByIdAsync(int id)
        {
            var materialHasSurfaceFinish = await _context.MaterialHasSurfaceFinish.FindAsync(id);
            return materialHasSurfaceFinish == null ? null : new MaterialHasSurfaceFinishDto { Id = materialHasSurfaceFinish.Id, MaterialId = materialHasSurfaceFinish.MaterialId, SurfaceFinishId = materialHasSurfaceFinish.SurfaceFinishId };
        }

        // SurfaceFinish operations
        public async Task<IEnumerable<SurfaceFinishDto>> GetAllSurfaceFinishesAsync()
        {
            var surfaceFinishes = await _context.SurfaceFinish.ToListAsync();
            return surfaceFinishes.Select(sf => new SurfaceFinishDto { Id = sf.Id, Name = sf.Name });
        }

        public async Task<SurfaceFinishDto?> GetSurfaceFinishByIdAsync(int id)
        {
            var surfaceFinish = await _context.SurfaceFinish.FindAsync(id);
            return surfaceFinish == null ? null : new SurfaceFinishDto { Id = surfaceFinish.Id, Name = surfaceFinish.Name };
        }

        public async Task<SurfaceFinishDto> CreateSurfaceFinishAsync(CreateSurfaceFinishRequest request)
        {
            var surfaceFinish = new SurfaceFinish { Name = request.Name };
            _context.SurfaceFinish.Add(surfaceFinish);
            await _context.SaveChangesAsync();
            return new SurfaceFinishDto { Id = surfaceFinish.Id, Name = surfaceFinish.Name };
        }

        public async Task<SurfaceFinishDto?> UpdateSurfaceFinishAsync(UpdateSurfaceFinishRequest request)
        {
            var surfaceFinish = await _context.SurfaceFinish.FindAsync(request.Id);
            if (surfaceFinish == null)
            {
                return null;
            }
            surfaceFinish.Name = request.Name;
            await _context.SaveChangesAsync();
            return new SurfaceFinishDto { Id = surfaceFinish.Id, Name = surfaceFinish.Name };
        }

        public async Task<bool> DeleteSurfaceFinishAsync(int id)
        {
            var surfaceFinish = await _context.SurfaceFinish.FindAsync(id);
            if (surfaceFinish == null)
            {
                return false;
            }
            _context.SurfaceFinish.Remove(surfaceFinish);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}