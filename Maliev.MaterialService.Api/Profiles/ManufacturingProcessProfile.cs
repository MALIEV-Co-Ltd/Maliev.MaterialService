using AutoMapper;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Profiles;

/// <summary>
/// AutoMapper profile for ManufacturingProcess entity mappings.
/// </summary>
public class ManufacturingProcessProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the ManufacturingProcessProfile class.
    /// </summary>
    public ManufacturingProcessProfile()
    {
        CreateMap<ManufacturingProcess, ManufacturingProcessDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<ManufacturingProcessCategory, ManufacturingProcessCategoryDto>();
    }
}