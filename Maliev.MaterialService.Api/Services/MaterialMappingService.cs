using AutoMapper;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

/// <summary>
/// Implementation of IMaterialMappingService using AutoMapper.
/// </summary>
public class MaterialMappingService : IMaterialMappingService
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the MaterialMappingService class.
    /// </summary>
    /// <param name="mapper">The AutoMapper instance.</param>
    public MaterialMappingService(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public MaterialDto MapToDto(Material material)
    {
        return _mapper.Map<MaterialDto>(material);
    }

    /// <inheritdoc/>
    public MaterialDto MapToDetailedDto(Material material)
    {
        return _mapper.Map<MaterialDto>(material);
    }

    /// <inheritdoc/>
    public IEnumerable<MaterialDto> MapToDtos(IEnumerable<Material> materials)
    {
        return _mapper.Map<IEnumerable<MaterialDto>>(materials);
    }

    /// <inheritdoc/>
    public Material MapFromCreateRequest(CreateMaterialRequest request)
    {
        return _mapper.Map<Material>(request);
    }

    /// <inheritdoc/>
    public Material MapFromUpdateRequest(UpdateMaterialRequest request, int id)
    {
        var material = _mapper.Map<Material>(request);
        material.Id = id;
        return material;
    }
}