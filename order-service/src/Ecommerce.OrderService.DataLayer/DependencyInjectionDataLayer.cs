using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Ecommerce.OrderService.DataLayer.RepositoryContacts;
using Ecommerce.OrderService.DataLayer.Repositories;
namespace Ecommerce.OrderService.DataLayer;

public static class DependencyInjectionDataLayer
{

    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringTemplate = configuration.GetConnectionString("MongoDbConnection")!;
        string mongoUser = Environment.GetEnvironmentVariable("MONGO_USER") ?? "defaultUser";
        string mongoPassword = Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? "defaultPassword";
        string mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "localhost";
        string mongoPort = Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017";
        string connectionString = connectionStringTemplate
             .Replace("{MONGO_USER}", mongoUser)
             .Replace("{MONGO_PASSWORD}", mongoPassword)
             .Replace("{MONGO_HOST}", mongoHost)
             .Replace("{MONGO_PORT}", mongoPort);
        // Console.WriteLine($"Generated Connection String: {connectionString}");
        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            IMongoClient client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
        });
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
    }
}
