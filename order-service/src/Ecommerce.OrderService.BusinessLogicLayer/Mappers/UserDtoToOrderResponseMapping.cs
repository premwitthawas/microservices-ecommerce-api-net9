using AutoMapper;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;

namespace Ecommerce.OrderService.BusinessLogicLayer.Mappers;

public class UserDtoToOrderResponseMapping : Profile
{
    public UserDtoToOrderResponseMapping()
    {
        CreateMap<UserDto, OrderResponse>()
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.UserPersonName, opt => opt.MapFrom(src => src.PersonName));
    }
}