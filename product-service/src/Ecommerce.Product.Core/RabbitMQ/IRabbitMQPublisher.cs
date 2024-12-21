namespace Ecommerce.Product.Core.RabbitMQ;
public interface IRabbitMQPublisher
{
    void Publish<T>(Dictionary<string, object> header, T message);
}