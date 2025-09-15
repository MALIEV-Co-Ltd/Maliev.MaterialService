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
}