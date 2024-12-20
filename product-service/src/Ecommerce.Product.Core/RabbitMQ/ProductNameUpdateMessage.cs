namespace Ecommerce.Product.Core.RabbitMQ;


public record ProductNameUpdateMessage(Guid ProductID, string? NewName);