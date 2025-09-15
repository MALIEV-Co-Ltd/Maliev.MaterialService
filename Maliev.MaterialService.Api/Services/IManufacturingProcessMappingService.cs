using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Interface for mapping ManufacturingProcess entities to DTOs and vice versa.
/// </summary>
public interface IManufacturingProcessMappingService
{
    /// <summary>
    /// Maps a ManufacturingProcess entity to a ManufacturingProcessDto.
    /// </summary>
    /// <param name="process">The ManufacturingProcess entity to map.</param>
    /// <returns>The mapped ManufacturingProcessDto.</returns>
    ManufacturingProcessDto MapToDto(ManufacturingProcess process);

    /// <summary>
    /// Maps a collection of ManufacturingProcess entities to ManufacturingProcessDtos.
    /// </summary>
    /// <param name="processes">The collection of ManufacturingProcess entities to map.</param>
    /// <returns>The mapped collection of ManufacturingProcessDtos.</returns>
    IEnumerable<ManufacturingProcessDto> MapToDtos(IEnumerable<ManufacturingProcess> processes);

    /// <summary>
    /// Maps a CreateManufacturingProcessRequest to a ManufacturingProcess entity.
    /// </summary>
    /// <param name="request">The CreateManufacturingProcessRequest to map.</param>
    /// <returns>The mapped ManufacturingProcess entity.</returns>
    ManufacturingProcess MapFromCreateRequest(CreateManufacturingProcessRequest request);

    /// <summary>
    /// Maps an UpdateManufacturingProcessRequest to a ManufacturingProcess entity.
    /// </summary>
    /// <param name="request">The UpdateManufacturingProcessRequest to map.</param>
    /// <param name="id">The ID of the ManufacturingProcess entity to update.</param>
    /// <returns>The mapped ManufacturingProcess entity.</returns>
    ManufacturingProcess MapFromUpdateRequest(UpdateManufacturingProcessRequest request, int id);
}