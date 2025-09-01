using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Data.Models; // For MaterialSortType if needed in service methods
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maliev.MaterialService.Api.Services
{
    public interface IMaterialServiceService
    {
        // Color operations
        Task<IEnumerable<ColorDto>> GetAllColorsAsync();
        Task<ColorDto?> GetColorByIdAsync(int id);
        Task<ColorDto> CreateColorAsync(CreateColorRequest request);
        Task<ColorDto?> UpdateColorAsync(UpdateColorRequest request);
        Task<bool> DeleteColorAsync(int id);

        // MaterialGroup operations
        Task<IEnumerable<MaterialGroupDto>> GetAllMaterialGroupsAsync();
        Task<MaterialGroupDto?> GetMaterialGroupByIdAsync(int id);
        Task<MaterialGroupDto> CreateMaterialGroupAsync(CreateMaterialGroupRequest request);
        Task<MaterialGroupDto?> UpdateMaterialGroupAsync(UpdateMaterialGroupRequest request);
        Task<bool> DeleteMaterialGroupAsync(int id);

        // Material operations
        Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync();
        Task<MaterialDto?> GetMaterialByIdAsync(int id);
        Task<MaterialDto> CreateMaterialAsync(CreateMaterialRequest request);
        
        Task<bool> DeleteMaterialAsync(int id);

                

        // MaterialHasColor operations
        Task<IEnumerable<MaterialHasColorDto>> GetAllMaterialHasColorsAsync();
        Task<MaterialHasColorDto?> GetMaterialHasColorByIdAsync(int id);
        // Add Create/Update/Delete methods for MaterialHasColor if needed, based on API requirements

        // MaterialHasSupplier operations
        Task<IEnumerable<MaterialHasSupplierDto>> GetAllMaterialHasSuppliersAsync();
        Task<MaterialHasSupplierDto?> GetMaterialHasSupplierByIdAsync(int id);
        // Add Create/Update/Delete methods for MaterialHasSupplier if needed

        // MaterialHasSurfaceFinish operations
        Task<IEnumerable<MaterialHasSurfaceFinishDto>> GetAllMaterialHasSurfaceFinishesAsync();
        Task<MaterialHasSurfaceFinishDto?> GetMaterialHasSurfaceFinishByIdAsync(int id);
        // Add Create/Update/Delete methods for MaterialHasSurfaceFinish if needed

        // SurfaceFinish operations
        Task<IEnumerable<SurfaceFinishDto>> GetAllSurfaceFinishesAsync();
        Task<SurfaceFinishDto?> GetSurfaceFinishByIdAsync(int id);
        Task<SurfaceFinishDto> CreateSurfaceFinishAsync(CreateSurfaceFinishRequest request);
        Task<SurfaceFinishDto?> UpdateSurfaceFinishAsync(UpdateSurfaceFinishRequest request);
        Task<bool> DeleteSurfaceFinishAsync(int id);
    }
}