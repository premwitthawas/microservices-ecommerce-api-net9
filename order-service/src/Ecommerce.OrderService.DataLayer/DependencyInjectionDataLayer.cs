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
        string connectionStringTempate = configuration.GetConnectionString("MongoDbConnection")!;
        string connectionString = connectionStringTempate.Replace("$MONGO_USER", Environment.GetEnvironmentVariable("MONGO_USER"))
        .Replace("$MONGO_PASSWORD", Environment.GetEnvironmentVariable("MONGO_PASSWORD"))
        .Replace("$MONGO_HOST", Environment.GetEnvironmentVariable("MONGO_HOST"))
        .Replace("$MONGO_PORT", Environment.GetEnvironmentVariable("MONGO_PORT"));
        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            IMongoClient client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase("ecommerceorderservicedb");
        });
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        return services;
    }
}
