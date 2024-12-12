using Ecommerce.Product.Infrastructure.Context;
using Ecommerce.Product.Infrastructure.Repositories;
using Ecommerce.Product.Infrastructure.RepositoryContacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Product.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration _configuration)
    {
        string connectionString = _configuration.GetConnectionString("MySqlConnection")!;
        string? host = Environment.GetEnvironmentVariable("MYSQL_HOST");
        string? port = Environment.GetEnvironmentVariable("MYSQL_PORT");
        string? password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
        string? user = Environment.GetEnvironmentVariable("MYSQL_USER");
        string? database = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
        connectionString = connectionString
        .Replace("{MYSQL_HOST}",host)
        .Replace("{MYSQL_PORT}",port)
        .Replace("{MYSQL_PASSWORD}",password)
        .Replace("{MYSQL_USER}",user)
        .Replace("{MYSQL_DATABASE}",database);
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySQL(connectionString));
        services.AddScoped<IProductsRepository, ProductsRepository>();
        return services;
    }

}