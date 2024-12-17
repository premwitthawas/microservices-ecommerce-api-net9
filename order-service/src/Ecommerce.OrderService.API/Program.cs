using Ecommerce.OrderService.API.Middlewares;
using Ecommerce.OrderService.BusinessLogicLayer;
using Ecommerce.OrderService.BusinessLogicLayer.HttpClients;
using Ecommerce.OrderService.BusinessLogicLayer.Policies;
using Ecommerce.OrderService.DataLayer;
using FluentValidation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddBusinessLogicLayer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
builder.Services.AddTransient<IUserMicroservicePolicies, UserMicroservicePolicies>();
builder.Services.AddTransient<IProductMicroservicePolicies, ProductMicroservicePolicies>();
builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
builder.Services.AddHttpClient<UsersMicroserviceClient>(clinet =>
{
    clinet.BaseAddress = new Uri($"http://{Environment.GetEnvironmentVariable("USERS_MICROSERVICE_HOST")}:{Environment.GetEnvironmentVariable("USERS_MICROSERVICE_PORT")}");
})
.AddPolicyHandler(builder.Services.BuildServiceProvider().GetRequiredService<IUserMicroservicePolicies>().GetCombinedPolicy());
// .AddPolicyHandler(builder.Services.BuildServiceProvider().GetRequiredService<IUserMicroservicePolicies>().GetRetryPolicy())
// .AddPolicyHandler(builder.Services.BuildServiceProvider().GetRequiredService<IUserMicroservicePolicies>().GetCircuitBreakerPolicy())
// .AddPolicyHandler(builder.Services.BuildServiceProvider().GetRequiredService<IUserMicroservicePolicies>().GetTimeoutPolicy());
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
builder.Services.AddHttpClient<ProductMicroserviceClient>(clinet =>
{
    clinet.BaseAddress = new Uri($"http://{Environment.GetEnvironmentVariable("PRODUCTS_MICROSERVICE_HOST")}:{Environment.GetEnvironmentVariable("PRODUCTS_MICROSERVICE_PORT")}");
})
.AddPolicyHandler(builder.Services.BuildServiceProvider().GetRequiredService<IProductMicroservicePolicies>().GetFallBackPolicy())
.AddPolicyHandler(builder.Services.BuildServiceProvider().GetRequiredService<IProductMicroservicePolicies>().GetBulkHeadIsolationPolicy());
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseErrorHandleMiddleware();
app.UseRouting();
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
