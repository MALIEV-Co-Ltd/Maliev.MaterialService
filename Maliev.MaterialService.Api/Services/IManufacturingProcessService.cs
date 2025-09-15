using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

public interface IManufacturingProcessService
{
    Task<IEnumerable<ManufacturingProcess>> GetAllProcessesAsync();
    Task<PagedResult<ManufacturingProcess>> GetAllProcessesPagedAsync(PaginationParameters pagination);
    Task<ManufacturingProcess?> GetProcessByIdAsync(int id);
    Task<IEnumerable<ManufacturingProcess>> GetProcessesByCategoryIdAsync(int categoryId);
    Task<PagedResult<ManufacturingProcess>> GetProcessesByCategoryIdPagedAsync(int categoryId, PaginationParameters pagination);
    Task<IEnumerable<ManufacturingProcessCategory>> GetAllCategoriesAsync();
    Task<ManufacturingProcessCategory?> GetCategoryByIdAsync(int id);
    Task<IEnumerable<Material>> GetCompatibleMaterialsAsync(int processId, int? compatibilityLevel = null);
    Task<ManufacturingProcess> CreateProcessAsync(ManufacturingProcess process);
    Task<ManufacturingProcess> UpdateProcessAsync(ManufacturingProcess process);
    Task DeleteProcessAsync(int id);
    Task<bool> ProcessExistsAsync(int id);
}