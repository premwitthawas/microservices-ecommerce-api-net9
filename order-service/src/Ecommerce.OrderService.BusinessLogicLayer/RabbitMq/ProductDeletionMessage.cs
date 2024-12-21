namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq
{
    public record ProductDeletionMessage(Guid ProductID, string ProductName);
}