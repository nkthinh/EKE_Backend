using AutoMapper;
using Repository.Entities;
using Service.DTO.Request;

namespace Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SubscriptionPackageCreateDto, SubscriptionPackage>();
            CreateMap<SubscriptionPackageUpdateDto, SubscriptionPackage>();
        }
    }
}
