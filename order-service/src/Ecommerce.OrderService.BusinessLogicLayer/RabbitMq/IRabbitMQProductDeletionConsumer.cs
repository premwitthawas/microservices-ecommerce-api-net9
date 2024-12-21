namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;

public interface IRabbitMQProductDeletionConsumer
{
    void Consume();
    void Dispose();
}