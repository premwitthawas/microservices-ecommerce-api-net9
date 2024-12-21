namespace Ecommerce.Product.Core.RabbitMQ
{
    public record ProductDeletionMessage(Guid ProductID,string? ProductName);
}