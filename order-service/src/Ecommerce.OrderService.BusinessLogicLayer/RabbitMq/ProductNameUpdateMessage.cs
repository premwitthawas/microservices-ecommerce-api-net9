namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;


public record ProductNameUpdateMessage(Guid ProductId, string? NewName);