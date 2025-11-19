using AutoMapper;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.MappingProfiles;

/// <summary>
/// AutoMapper profile for material-related mappings
/// </summary>
public class MaterialProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of MaterialProfile
    /// </summary>
    public MaterialProfile()
    {
        // Entity to Response
        CreateMap<Material, MaterialResponse>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.ManufacturingProcesses, opt => opt.MapFrom(src => src.ManufacturingProcesses))
            .ForMember(dest => dest.AvailableColors, opt => opt.MapFrom(src => src.AvailableColors))
            .ForMember(dest => dest.PostProcessingMethods, opt => opt.MapFrom(src => src.PostProcessingMethods))
            .ForMember(dest => dest.MechanicalProperties, opt => opt.MapFrom(src => src.MechanicalProperties));

        CreateMap<ManufacturingProcess, ManufacturingProcessResponse>();
        CreateMap<Color, ColorResponse>();
        CreateMap<PostProcessingMethod, PostProcessingMethodResponse>();

        CreateMap<MaterialMechanicalProperty, MaterialMechanicalPropertyResponse>()
            .ForMember(dest => dest.MechanicalPropertyName, opt => opt.MapFrom(src => src.MechanicalProperty.Name))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.MechanicalProperty.Unit));

        // Request to Entity
        CreateMap<CreateMaterialRequest, Material>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Version, opt => opt.Ignore())
            .ForMember(dest => dest.Active, opt => opt.Ignore())
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.ManufacturingProcesses, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableColors, opt => opt.Ignore())
            .ForMember(dest => dest.PostProcessingMethods, opt => opt.Ignore())
            .ForMember(dest => dest.MechanicalProperties, opt => opt.Ignore());

        CreateMap<UpdateMaterialRequest, Material>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Active, opt => opt.Ignore())
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.ManufacturingProcesses, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableColors, opt => opt.Ignore())
            .ForMember(dest => dest.PostProcessingMethods, opt => opt.Ignore())
            .ForMember(dest => dest.MechanicalProperties, opt => opt.Ignore());

        CreateMap<MaterialMechanicalPropertyRequest, MaterialMechanicalProperty>()
            .ForMember(dest => dest.MaterialId, opt => opt.Ignore())
            .ForMember(dest => dest.Material, opt => opt.Ignore())
            .ForMember(dest => dest.MechanicalProperty, opt => opt.Ignore());
    }
}
