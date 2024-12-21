using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;


public class RabbitMQProductDeletionConsumer : IRabbitMQProductDeletionConsumer, IDisposable
{

    private readonly IConfiguration _configuration;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQProductDeletionConsumer> _logger;
    private readonly IDistributedCache _distributedCache;
    public RabbitMQProductDeletionConsumer(IConfiguration configuration, ILogger<RabbitMQProductDeletionConsumer> logger, IDistributedCache distributedCache)
    {
        _configuration = configuration;
        _logger = logger;
        string rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")!;
        string rabbitMqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT")!;
        string rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER")!;
        string rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!;
        ConnectionFactory factory = new()
        {
            HostName = rabbitMqHost,
            Port = Convert.ToInt32(rabbitMqPort),
            UserName = rabbitMqUser,
            Password = rabbitMqPass
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _distributedCache = distributedCache;
    }
    public void Consume()
    {
        _logger.LogInformation("Product Deletion Consumer listening for messages");
        string routeKey = Environment.GetEnvironmentVariable("RABBITMQ_CONNSUMMER_DELPRODUCT_KEY")!;
        string queueName = Environment.GetEnvironmentVariable("RABBITMQ_CONNSUMMER_DELPRODUCT_QUEUE")!;
        string exchangeName = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE")!;
        // Declare Exchange
        _channel.ExchangeDeclare(exchange: exchangeName,
        type: ExchangeType.Headers, durable: true);
        // Declare Queue
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        // Bind Queue With Exchange
        var header = new Dictionary<string, object>
            {
                {
                    "x-match","any"
                },
                {
                    "event",routeKey
                },
                {
                    "field","all"
                },
                {
                    "rowCount",1
                }
            };
        _channel.QueueBind(queue: queueName, exchange: exchangeName,
        routingKey: string.Empty, arguments: header);
        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (sender, args) =>
        {
            byte[] body = args.Body.ToArray();
            string msg = Encoding.UTF8.GetString(body);
            // _logger.LogInformation($"Product Deletion Consumer Received : {msg}");
            if (msg != null)
            {
                ProductDeletionMessage? productDeletionMessage = JsonSerializer.Deserialize<ProductDeletionMessage>(msg);
                if (productDeletionMessage != null)
                {
                    //_logger.LogInformation($"Product Deletion Consumer Received : {productDeletionMessage.ProductID}");
                   await this.DeleteProductNameOnBackGroupd(productDeletionMessage.ProductID);
                }
            }
        };
        _channel.BasicConsume(queue: queueName, consumer: consumer, autoAck: true);
    }

    private async Task DeleteProductNameOnBackGroupd(Guid productID)
    {
        _logger.LogInformation($"Product DeleteById : {productID}");
        string cachekey = $"product:{productID}";
        await _distributedCache.RemoveAsync(cachekey);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
};