using Repository.Entities;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Service.DTO;
using Repository.Entities;

namespace Service.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<User, UserResponseDto>();
        }
    }
}