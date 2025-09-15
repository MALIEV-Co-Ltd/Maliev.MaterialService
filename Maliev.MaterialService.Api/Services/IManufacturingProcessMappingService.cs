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
}