using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Interface for mapping MaterialGroup entities to DTOs and vice versa.
/// </summary>
public interface IMaterialGroupMappingService
{
    /// <summary>
    /// Maps a MaterialGroup entity to a MaterialGroupDto.
    /// </summary>
    /// <param name="group">The MaterialGroup entity to map.</param>
    /// <returns>The mapped MaterialGroupDto.</returns>
    MaterialGroupDto MapToDto(MaterialGroup group);

    /// <summary>
    /// Maps a collection of MaterialGroup entities to MaterialGroupDtos.
    /// </summary>
    /// <param name="groups">The collection of MaterialGroup entities to map.</param>
    /// <returns>The mapped collection of MaterialGroupDtos.</returns>
    IEnumerable<MaterialGroupDto> MapToDtos(IEnumerable<MaterialGroup> groups);

    /// <summary>
    /// Maps a CreateMaterialGroupRequest to a MaterialGroup entity.
    /// </summary>
    /// <param name="request">The CreateMaterialGroupRequest to map.</param>
    /// <returns>The mapped MaterialGroup entity.</returns>
    MaterialGroup MapFromCreateRequest(CreateMaterialGroupRequest request);

    /// <summary>
    /// Maps an UpdateMaterialGroupRequest to a MaterialGroup entity.
    /// </summary>
    /// <param name="request">The UpdateMaterialGroupRequest to map.</param>
    /// <param name="id">The ID of the MaterialGroup entity to update.</param>
    /// <returns>The mapped MaterialGroup entity.</returns>
    MaterialGroup MapFromUpdateRequest(UpdateMaterialGroupRequest request, int id);
}