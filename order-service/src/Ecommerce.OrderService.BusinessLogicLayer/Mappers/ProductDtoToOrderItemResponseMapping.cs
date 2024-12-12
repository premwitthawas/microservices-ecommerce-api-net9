using AutoMapper;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;

namespace Ecommerce.OrderService.BusinessLogicLayer.Mappers;

public class ProductDtoToOrderItemResponseMapping: Profile {
    public ProductDtoToOrderItemResponseMapping()
    {
        CreateMap<ProductDto,OrderItemResponse>()
        .ForMember(dest=>dest.ProductName,opt=>opt.MapFrom(src=>src.ProductName))
        .ForMember(dest=>dest.Category,opt=>opt.MapFrom(src=>src.Category));
    }
}