using AutoMapper;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Implementation of IManufacturingProcessMappingService using AutoMapper.
/// </summary>
public class ManufacturingProcessMappingService : IManufacturingProcessMappingService
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the ManufacturingProcessMappingService class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    public ManufacturingProcessMappingService(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public ManufacturingProcessDto MapToDto(ManufacturingProcess process)
    {
        return _mapper.Map<ManufacturingProcessDto>(process);
    }

    /// <inheritdoc/>
    public IEnumerable<ManufacturingProcessDto> MapToDtos(IEnumerable<ManufacturingProcess> processes)
    {
        return _mapper.Map<IEnumerable<ManufacturingProcessDto>>(processes);
    }

    /// <inheritdoc/>
    public ManufacturingProcess MapFromCreateRequest(CreateManufacturingProcessRequest request)
    {
        return _mapper.Map<ManufacturingProcess>(request);
    }

    /// <inheritdoc/>
    public ManufacturingProcess MapFromUpdateRequest(UpdateManufacturingProcessRequest request, int id)
    {
        var process = _mapper.Map<ManufacturingProcess>(request);
        process.Id = id;
        return process;
    }
}