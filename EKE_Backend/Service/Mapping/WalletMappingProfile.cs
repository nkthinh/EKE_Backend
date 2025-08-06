using AutoMapper;
using Repository.Entities;
using Service.DTO.Request;
using Service.DTO.Response;

namespace Service.Mappings
{
    public class WalletMappingProfile : Profile
    {
        public WalletMappingProfile()
        {
            // Map từ Entity -> Response DTO
            CreateMap<Wallet, WalletResponseDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            // Map từ Request DTO -> Entity
            CreateMap<WalletRequestDto, Wallet>();
        }
    }
}
