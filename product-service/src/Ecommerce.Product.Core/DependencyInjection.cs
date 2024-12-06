using Ecommerce.Product.Core.Mappers;
using Ecommerce.Product.Core.ServiceContacts;
using Ecommerce.Product.Core.Services;
using Ecommerce.Product.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Product.Core;

public static class DependencyInjection
{

    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingCreateProductRequestProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
        services.AddScoped<IProductsService, ProductsService>();
        return services;
    }

}