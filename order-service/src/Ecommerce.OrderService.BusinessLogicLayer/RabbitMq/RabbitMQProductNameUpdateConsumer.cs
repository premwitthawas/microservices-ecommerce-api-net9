using System.Text;
using System.Text.Json;
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
    public RabbitMQProductNameUpdateConsumer(IConfiguration configuration, IModel channel, IConnection connection, ILogger<RabbitMQProductNameUpdateConsumer> logger)
    {
        _configuration = configuration;
        string rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")!;
        string rabbitMqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT")!;
        string rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER")!;
        string rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!;
        ConnectionFactory factory = new()
        {
            HostName = rabbitMqHost,
            Port = Convert.ToInt16(rabbitMqPort),
            UserName = rabbitMqUser,
            Password = rabbitMqPass
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _logger = logger;
    }
    public void Consume()
    {
        _logger.LogInformation("Consume");
        string routeKey = Environment.GetEnvironmentVariable("RABBITMQ_CONNSUMMER_PRODUCTNAME_KEY")!;
        string queueName = Environment.GetEnvironmentVariable("RABBITMQ_CONNSUMMER_PRODUCTNAME_QUEUE")!;
        string exchangeName = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE")!;
        // Declare Queue
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        // Bind Queue With Exchange
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routeKey);
        _logger.LogInformation(" [*] Waiting for logs.");
        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += (sender, args) =>
        {
            byte[] body = args.Body.ToArray();
            string msg = Encoding.UTF8.GetString(body);
            if (msg != null)
            {
                ProductNameUpdateMessage productNameUpdate = JsonSerializer.Deserialize<ProductNameUpdateMessage>(msg)!;
                _logger.LogInformation($"Product Name Update Message Received: {productNameUpdate.ProductId} - {productNameUpdate.NewName}");
            }
        };
        _channel.BasicConsume(queue: queueName, consumer: consumer, autoAck: true);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
};