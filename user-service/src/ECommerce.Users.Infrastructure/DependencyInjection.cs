using ECommerce.Users.Core.RepositoryContacts;
using ECommerce.Users.Infrastructure.DbContext;
using ECommerce.Users.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Users.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method to add infrastructure services to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        string? host = Environment.GetEnvironmentVariable("PG_HOST");
        int? port = int.Parse(Environment.GetEnvironmentVariable("PG_PORT")!);
        string? username = Environment.GetEnvironmentVariable("PG_USER");
        string? password = Environment.GetEnvironmentVariable("PG_PASSWORD");
        string? database = Environment.GetEnvironmentVariable("PG_DB");
        Console.WriteLine($"Host: {host}, Port: {port}, Username: {username}, Password: {password}, Database: {database}");
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<DapperDbContext>();
        return services;
    }
}
