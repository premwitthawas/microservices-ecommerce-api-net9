namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;

public interface IRabbitMQProductNameUpdateConsumer
{
    void Consume();
    void Dispose();
}