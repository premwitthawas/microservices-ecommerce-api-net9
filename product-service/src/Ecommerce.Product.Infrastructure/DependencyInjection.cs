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
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySQL(_configuration.GetConnectionString("MySqlConnection")!));
        services.AddScoped<IProductsRepository, ProductsRepository>();
        return services;
    }

}