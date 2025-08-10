using AutoMapper;
using Repository.Entities;
using Service.DTO.Response;

public class MappingProfile2 : Profile
{
    public MappingProfile2()
    {
        CreateMap<SubscriptionPackage, SubscriptionPackageResponseDto>();
        // Nếu muốn map ngược
        // CreateMap<SubscriptionPackageResponseDto, SubscriptionPackage>();
    }
}
