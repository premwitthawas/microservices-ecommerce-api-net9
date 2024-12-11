using AutoMapper;
using ECommerce.Users.Core.DTOs;
using ECommerce.Users.Core.Entities;

namespace ECommerce.Users.Core.AutoMappers;


public class ApplicationUserToUserDtoMappingProfile : Profile
{
    public ApplicationUserToUserDtoMappingProfile()
    {
        CreateMap<ApplicaitonUser, UserDto>()
        .ForMember(des => des.UserID, opt => opt.MapFrom(t => t.UserID))
        .ForMember(des => des.Email, opt => opt.MapFrom(t => t.Email))
        .ForMember(des => des.Gender, opt => opt.MapFrom(t => t.Gender))
        .ForMember(des => des.PersonName, opt => opt.MapFrom(t => t.PersonName));
    }
}