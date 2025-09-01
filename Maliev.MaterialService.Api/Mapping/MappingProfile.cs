using AutoMapper;
using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Data.Models;

namespace Maliev.MaterialService.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Color, ColorDto>();
            CreateMap<MaterialGroup, MaterialGroupDto>();
            CreateMap<Material, MaterialDto>();
            CreateMap<MaterialHasColor, MaterialHasColorDto>();
            CreateMap<MaterialHasSupplier, MaterialHasSupplierDto>();
            CreateMap<MaterialHasSurfaceFinish, MaterialHasSurfaceFinishDto>();
            CreateMap<SurfaceFinish, SurfaceFinishDto>();

            // Request DTO to Entity mappings
            CreateMap<CreateColorRequest, Color>();
            CreateMap<UpdateColorRequest, Color>();
            CreateMap<CreateMaterialGroupRequest, MaterialGroup>();
            CreateMap<UpdateMaterialGroupRequest, MaterialGroup>();
            CreateMap<CreateMaterialRequest, Material>();
            CreateMap<UpdateMaterialRequest, Material>();
            CreateMap<CreateSurfaceFinishRequest, SurfaceFinish>();
            CreateMap<UpdateSurfaceFinishRequest, SurfaceFinish>();
        }
    }
}