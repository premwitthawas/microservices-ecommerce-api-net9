namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;

public interface IRabbitMQProductNameUpdateConsumer : IDisposable
{
    void Consume();
}