using System.Text;
using System.Text.Json;
using Ecommerce.OrderService.BusinessLogicLayer.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;


public class RabbitMQProductNameUpdateConsumer : IRabbitMQProductNameUpdateConsumer, IDisposable
{

    private readonly IConfiguration _configuration;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQProductNameUpdateConsumer> _logger;
    private readonly IDistributedCache _distributedCache;
    public RabbitMQProductNameUpdateConsumer(IConfiguration configuration, ILogger<RabbitMQProductNameUpdateConsumer> logger, IDistributedCache distributedCache)
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
        _logger.LogInformation("Product Name Update Consumer listening for messages");
        string routeKey = Environment.GetEnvironmentVariable("RABBITMQ_CONNSUMMER_PRODUCTNAME_KEY")!;
        string queueName = Environment.GetEnvironmentVariable("RABBITMQ_CONNSUMMER_PRODUCTNAME_QUEUE")!;
        string exchangeName = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE")!;
        // Declare Exchange
        _channel.ExchangeDeclare(exchange: exchangeName, type:
        ExchangeType.Headers, durable: true);
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
                    "field","name"
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
            _logger.LogInformation($"Product Name Updated {msg}");
            if (msg != null)
            {
                ProductDto? productDto = JsonSerializer.Deserialize<ProductDto>(msg);
                if (productDto != null)
                {
                    // _logger.LogInformation($"Product Name Updated ID {productDto.ProductID} : {productDto.ProductName}");
                    await UpdateProductNameOnBackGroupd(productDto);
                }
            }
        };
        _channel.BasicConsume(queue: queueName, consumer: consumer, autoAck: true);
    }

    private async Task UpdateProductNameOnBackGroupd(ProductDto productDto)
    {
        _logger.LogInformation($"Product Name Updated ID {productDto.ProductID} : {productDto.ProductName}");
        string productJson = JsonSerializer.Serialize(productDto);
        string cachekey = $"product:{productDto.ProductID}";
        await _distributedCache.SetStringAsync(cachekey, productJson);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
};