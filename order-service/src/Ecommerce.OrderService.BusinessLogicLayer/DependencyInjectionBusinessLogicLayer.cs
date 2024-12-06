using Ecommerce.OrderService.BusinessLogicLayer.Mappers;
using Ecommerce.OrderService.BusinessLogicLayer.ServiceContacts;
using Ecommerce.OrderService.BusinessLogicLayer.Services;
using Ecommerce.OrderService.BusinessLogicLayer.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.OrderService.BusinessLogicLayer;

public static class DependencyInjectionBusinessLogicLayer
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
        services.AddAutoMapper(typeof(OrderItemAddRequestToOrderItemMapping).Assembly);
        return services;
    }
}
