using AutoMapper;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.DataLayer.Models;

namespace Ecommerce.OrderService.BusinessLogicLayer.Mappers;

public class OrderToOrderResponseMapping : Profile
{
    public OrderToOrderResponseMapping()
    {
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
            .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderID))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(src => src.TotalBill));
    }
}