using AutoMapper;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Profiles;

/// <summary>
/// AutoMapper profile for Material entity mappings.
/// </summary>
public class MaterialProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MaterialProfile class.
    /// </summary>
    public MaterialProfile()
    {
        CreateMap<Material, MaterialDto>()
            .ForMember(dest => dest.MaterialGroup, opt => opt.MapFrom(src => src.MaterialGroup))
            .ForMember(dest => dest.MaterialStandards, opt => opt.MapFrom(src => src.MaterialStandards))
            .ForMember(dest => dest.MaterialProperties, opt => opt.MapFrom(src => src.MaterialProperties))
            .ForMember(dest => dest.ProcessCompatibilities, opt => opt.MapFrom(src => src.ProcessCompatibilities))
            .ForMember(dest => dest.MaterialColors, opt => opt.MapFrom(src => src.MaterialColors))
            .ForMember(dest => dest.MaterialSurfaceFinishes, opt => opt.MapFrom(src => src.MaterialSurfaceFinishes));

        CreateMap<MaterialGroup, MaterialGroupDto>()
            .ForMember(dest => dest.MaterialFamily, opt => opt.MapFrom(src => src.MaterialFamily));

        CreateMap<MaterialFamily, MaterialFamilyDto>();

        CreateMap<MaterialStandard, MaterialStandardDto>()
            .ForMember(dest => dest.StandardType, opt => opt.MapFrom(src => src.StandardType));

        CreateMap<MaterialStandardType, MaterialStandardTypeDto>();

        CreateMap<MaterialProperty, MaterialPropertyDto>()
            .ForMember(dest => dest.PropertySubtype, opt => opt.MapFrom(src => src.PropertySubtype));

        CreateMap<PropertySubtype, PropertySubtypeDto>()
            .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType));

        CreateMap<PropertyType, PropertyTypeDto>();

        CreateMap<MaterialProcessCompatibility, MaterialProcessCompatibilityDto>()
            .ForMember(dest => dest.Process, opt => opt.MapFrom(src => src.Process));

        CreateMap<ManufacturingProcess, ManufacturingProcessDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<ManufacturingProcessCategory, ManufacturingProcessCategoryDto>();

        CreateMap<MaterialColor, MaterialColorDto>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color));

        CreateMap<Color, ColorDto>();

        CreateMap<MaterialSurfaceFinish, MaterialSurfaceFinishDto>()
            .ForMember(dest => dest.SurfaceFinish, opt => opt.MapFrom(src => src.SurfaceFinish));

        CreateMap<SurfaceFinish, SurfaceFinishDto>();

        CreateMap<CreateMaterialRequest, Material>();

        CreateMap<UpdateMaterialRequest, Material>();
    }
}