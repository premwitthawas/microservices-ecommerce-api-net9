using Ecommerce.OrderService.DataLayer.Models;
using Ecommerce.OrderService.DataLayer.RepositoryContacts;
using MongoDB.Driver;

namespace Ecommerce.OrderService.DataLayer.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<Order> _ordersCollection;
    private readonly string _collectionName = "orders";
    public OrdersRepository(IMongoDatabase database)
    {
        _ordersCollection = database.GetCollection<Order>(_collectionName);
    }
    public async Task<Order?> AddOrderAsync(Order order)
    {
        order.OrderID = Guid.NewGuid();
        order._id = order.OrderID;
        foreach (OrderItem orderItem in order.OrderItems)
        {
            orderItem._id = Guid.NewGuid();
        }
        await _ordersCollection.InsertOneAsync(order);
        return order;
    }

    public async Task<bool> DelelteOrderAsync(Guid id)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, id);
        Order? existingOrder = (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
        if (existingOrder == null)
        {
            return false;
        }
        DeleteResult result = await _ordersCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<Order?> GetOrderByCondition(FilterDefinition<Order> filter)
    {
        return (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return (await _ordersCollection.FindAsync(Builders<Order>.Filter.Empty)).ToList();
    }

    public async Task<IEnumerable<Order?>> GetOrdersByCondition(FilterDefinition<Order> filter)
    {
        return (await _ordersCollection.FindAsync(filter)).ToList();
    }

    public async Task<Order?> UpdateOrderAsync(Order update)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.OrderID, update.OrderID);
        Order? existingOrder = (await _ordersCollection.FindAsync(filter)).FirstOrDefault();
        if (existingOrder == null)
        {
            return null;
        }
        update._id = existingOrder._id;
        ReplaceOneResult result = await _ordersCollection.ReplaceOneAsync(filter, update);
        return update;
    }
};