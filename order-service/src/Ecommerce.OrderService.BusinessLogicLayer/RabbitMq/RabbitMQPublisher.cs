using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Ecommerce.OrderService.BusinessLogicLayer.RabbitMq;


public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    public RabbitMQPublisher(IConfiguration configuration)
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
        _connection =  factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }

    public void Publish<T>(string routeKey, T message)
    {
        string messageJson = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(messageJson);
        string exchangeName = "products.exchange";
        // Declare exchange
        _channel.ExchangeDeclare(exchange: exchangeName,type: ExchangeType.Direct,durable: true);
        // Publish message
        _channel.BasicPublish(exchange: exchangeName, routingKey: routeKey, basicProperties: null, body: body);
    }
}