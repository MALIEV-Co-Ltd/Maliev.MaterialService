using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Interface for mapping Material entities to DTOs and vice versa.
/// </summary>
public interface IMaterialMappingService
{
    /// <summary>
    /// Maps a Material entity to a MaterialDto.
    /// </summary>
    /// <param name="material">The Material entity to map.</param>
    /// <returns>The mapped MaterialDto.</returns>
    MaterialDto MapToDto(Material material);

    /// <summary>
    /// Maps a Material entity to a detailed MaterialDto.
    /// </summary>
    /// <param name="material">The Material entity to map.</param>
    /// <returns>The mapped detailed MaterialDto.</returns>
    MaterialDto MapToDetailedDto(Material material);

    /// <summary>
    /// Maps a collection of Material entities to MaterialDtos.
    /// </summary>
    /// <param name="materials">The collection of Material entities to map.</param>
    /// <returns>The mapped collection of MaterialDtos.</returns>
    IEnumerable<MaterialDto> MapToDtos(IEnumerable<Material> materials);

    /// <summary>
    /// Maps a CreateMaterialRequest to a Material entity.
    /// </summary>
    /// <param name="request">The CreateMaterialRequest to map.</param>
    /// <returns>The mapped Material entity.</returns>
    Material MapFromCreateRequest(CreateMaterialRequest request);

    /// <summary>
    /// Maps an UpdateMaterialRequest to a Material entity.
    /// </summary>
    /// <param name="request">The UpdateMaterialRequest to map.</param>
    /// <param name="id">The ID to set on the mapped entity.</param>
    /// <returns>The mapped Material entity.</returns>
    Material MapFromUpdateRequest(UpdateMaterialRequest request, int id);
}