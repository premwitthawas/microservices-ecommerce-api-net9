using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Ecommerce.Product.Core.RabbitMQ;
public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQPublisher> _logger;
    public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
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

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

    public void Publish<T>(string routeKey, T message)
    {
        _logger.LogInformation("Publishing message to RabbitMQ");
        string messageJson = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(messageJson);
        string exchangeName = Environment.GetEnvironmentVariable("RABBITMQ_EXCHANGE")!;
        // Declare exchange
        _channel.ExchangeDeclare(exchange: exchangeName,type: ExchangeType.Direct,durable: true);
        // Publish message
        _channel.BasicPublish(exchange: exchangeName, routingKey: routeKey, basicProperties: null, body: body);
        _logger.LogInformation(messageJson);
    }
}