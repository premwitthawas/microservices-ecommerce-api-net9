using ECommerce.Users.Core.ServiceContacts;
using ECommerce.Users.Core.Services;
using ECommerce.Users.Core.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
namespace ECommerce.Users.Core;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method to add Core services to the DI container
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IUsersService, UsersService>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidation>();
        return services;
    }
}
