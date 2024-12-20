namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;

public interface IRabbitMQPublisher
{
    void Publish<T>(string routeKey, T message);
}