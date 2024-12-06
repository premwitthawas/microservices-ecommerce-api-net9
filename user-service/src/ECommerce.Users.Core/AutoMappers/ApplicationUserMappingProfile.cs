using AutoMapper;
using ECommerce.Users.Core.DTOs;
using ECommerce.Users.Core.Entities;

namespace ECommerce.Users.Core.AutoMappers;

public class ApplicationUserMappingProfile : Profile
{
    public ApplicationUserMappingProfile()
    {
        CreateMap<AppplicaitonUser, AuthenticationResponse>()
        .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.PersonName))
        .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
        .ForMember(dest => dest.Token, opt => opt.Ignore())
        .ForMember(dest => dest.Success, opt => opt.Ignore());
    }
}