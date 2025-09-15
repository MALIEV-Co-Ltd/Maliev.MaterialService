using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;
using System.Threading;

namespace Maliev.MaterialService.Api.Services;

public interface IMaterialGroupService
{
    Task<IEnumerable<MaterialGroup>> GetAllGroupsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<MaterialGroup>> GetAllGroupsPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
    Task<MaterialGroup?> GetGroupByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaterialGroup>> GetGroupsByFamilyIdAsync(int familyId, CancellationToken cancellationToken = default);
    Task<PagedResult<MaterialGroup>> GetGroupsByFamilyIdPagedAsync(int familyId, PaginationParameters pagination, CancellationToken cancellationToken = default);
    Task<MaterialGroup> CreateGroupAsync(MaterialGroup group, CancellationToken cancellationToken = default);
    Task<MaterialGroup> UpdateGroupAsync(MaterialGroup group, CancellationToken cancellationToken = default);
    Task DeleteGroupAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> GroupExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaterialFamily>> GetAllFamiliesAsync(CancellationToken cancellationToken = default);
    Task<MaterialFamily?> GetFamilyByIdAsync(int id, CancellationToken cancellationToken = default);
}