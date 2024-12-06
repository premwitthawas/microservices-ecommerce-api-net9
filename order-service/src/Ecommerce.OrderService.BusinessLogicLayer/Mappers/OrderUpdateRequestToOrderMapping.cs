using AutoMapper;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.DataLayer.Models;

namespace Ecommerce.OrderService.BusinessLogicLayer.Mappers;

class OrderUpdateRequestToOrderMapping : Profile
{
    public OrderUpdateRequestToOrderMapping()
    {
        CreateMap<OrderUpdateRequest, Order>()
        .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderID))
        .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
        .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
        .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
        .ForMember(dest => dest.TotalBill, opt => opt.Ignore())
        .ForMember(dest => dest._id, opt => opt.Ignore());
    }
}