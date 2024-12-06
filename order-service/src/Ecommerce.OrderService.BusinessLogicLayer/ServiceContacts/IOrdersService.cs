using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Ecommerce.OrderService.DataLayer.Models;
using MongoDB.Driver;

namespace Ecommerce.OrderService.BusinessLogicLayer.ServiceContacts;

public interface IOrdersService
{
    Task<OrderResponse?> AddOrderAsync(OrderAddRequest orderAddRequest);
    Task<OrderResponse?> GetOrderByCondtitionAsync(FilterDefinition<Order> filter);
    Task<List<OrderResponse?>> GetOrdersByCondtitionAsync(FilterDefinition<Order> filter);
    Task<List<OrderResponse?>> GetOrdersAsync();
    Task<OrderResponse?> UpdateOrderAsync(OrderUpdateRequest orderUpdateRequest);
    Task<bool> DeleteOrderAsync(Guid orderID);
}