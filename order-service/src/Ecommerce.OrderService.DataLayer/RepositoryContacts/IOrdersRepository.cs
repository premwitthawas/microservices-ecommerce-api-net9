using Ecommerce.OrderService.DataLayer.Models;
using MongoDB.Driver;

namespace Ecommerce.OrderService.DataLayer.RepositoryContacts
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter);
        Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter);
        Task<Order?> AddOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(Order update);
        Task<bool> DelelteOrderAsync(Guid id);
    }
}