using AutoMapper;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.DataLayer.Models;

namespace Ecommerce.OrderService.BusinessLogicLayer.Mappers;

public class OrderItemAddRequestToOrderItemMapping : Profile
{
    public OrderItemAddRequestToOrderItemMapping()
    {
        CreateMap<OrderItemAddRequest, OrderItem>()
        .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
        .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
        .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
        .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
        .ForMember(dest => dest._id, opt => opt.Ignore());
    }
};