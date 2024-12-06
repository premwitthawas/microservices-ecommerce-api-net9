namespace Ecommerce.OrderService.BusinessLogicLayer.Mappers
{
    using AutoMapper;
    using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
    using Ecommerce.OrderService.DataLayer.Models;

    public class OrderItemUpdateToOrderItemMapping : Profile
    {
        public OrderItemUpdateToOrderItemMapping()
        {
            CreateMap<OrderItemUpdateRequest, OrderItem>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }
    }
}