using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

public interface IMaterialGroupService
{
    Task<IEnumerable<MaterialGroup>> GetAllGroupsAsync();
    Task<MaterialGroup?> GetGroupByIdAsync(int id);
    Task<IEnumerable<MaterialGroup>> GetGroupsByFamilyIdAsync(int familyId);
    Task<MaterialGroup> CreateGroupAsync(MaterialGroup group);
    Task<MaterialGroup> UpdateGroupAsync(MaterialGroup group);
    Task DeleteGroupAsync(int id);
    Task<bool> GroupExistsAsync(int id);
    Task<IEnumerable<MaterialFamily>> GetAllFamiliesAsync();
    Task<MaterialFamily?> GetFamilyByIdAsync(int id);
}