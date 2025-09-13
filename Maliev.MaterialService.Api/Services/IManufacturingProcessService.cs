using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

public interface IManufacturingProcessService
{
    Task<IEnumerable<ManufacturingProcess>> GetAllProcessesAsync();
    Task<ManufacturingProcess?> GetProcessByIdAsync(int id);
    Task<IEnumerable<ManufacturingProcess>> GetProcessesByCategoryIdAsync(int categoryId);
    Task<IEnumerable<ManufacturingProcessCategory>> GetAllCategoriesAsync();
    Task<ManufacturingProcessCategory?> GetCategoryByIdAsync(int id);
    Task<IEnumerable<Material>> GetCompatibleMaterialsAsync(int processId, int? compatibilityLevel = null);
}