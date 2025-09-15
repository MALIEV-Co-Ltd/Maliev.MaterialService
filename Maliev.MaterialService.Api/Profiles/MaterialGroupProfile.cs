using AutoMapper;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Profiles;

/// <summary>
/// AutoMapper profile for MaterialGroup entity mappings.
/// </summary>
public class MaterialGroupProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MaterialGroupProfile class.
    /// </summary>
    public MaterialGroupProfile()
    {
        CreateMap<MaterialGroup, MaterialGroupDto>()
            .ForMember(dest => dest.MaterialFamily, opt => opt.MapFrom(src => src.MaterialFamily));

        CreateMap<MaterialFamily, MaterialFamilyDto>();
    }
}