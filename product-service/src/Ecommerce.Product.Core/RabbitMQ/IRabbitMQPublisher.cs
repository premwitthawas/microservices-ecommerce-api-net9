namespace Ecommerce.Product.Core.RabbitMQ;
public interface IRabbitMQPublisher
{
    void Publish<T>(string routeKey, T message);
}