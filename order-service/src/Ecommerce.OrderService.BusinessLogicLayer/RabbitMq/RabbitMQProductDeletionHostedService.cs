using Microsoft.Extensions.Hosting;

namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;


public class RabbitMQProductDeletionHostedService : IHostedService
{
    private readonly IRabbitMQProductDeletionConsumer _rabbitMQProductNameUpdateConsumer;
    public RabbitMQProductDeletionHostedService(IRabbitMQProductDeletionConsumer rabbitMQProductDeletion)
    {
        _rabbitMQProductNameUpdateConsumer = rabbitMQProductDeletion;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMQProductNameUpdateConsumer.Consume();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _rabbitMQProductNameUpdateConsumer.Dispose();
        return Task.CompletedTask;
    }
}