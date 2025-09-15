using AutoMapper;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Implementation of IMaterialGroupMappingService using AutoMapper.
/// </summary>
public class MaterialGroupMappingService : IMaterialGroupMappingService
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the MaterialGroupMappingService class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    public MaterialGroupMappingService(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public MaterialGroupDto MapToDto(MaterialGroup group)
    {
        return _mapper.Map<MaterialGroupDto>(group);
    }

    /// <inheritdoc/>
    public IEnumerable<MaterialGroupDto> MapToDtos(IEnumerable<MaterialGroup> groups)
    {
        return _mapper.Map<IEnumerable<MaterialGroupDto>>(groups);
    }
}