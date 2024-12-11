using DotNetEnv;
using Ecommerce.OrderService.API.Middlewares;
using Ecommerce.OrderService.BusinessLogicLayer;
using Ecommerce.OrderService.BusinessLogicLayer.HttpClients;
using Ecommerce.OrderService.DataLayer;
using FluentValidation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    Env.Load();
}
// Console.WriteLine($"MONGO_USER: {Environment.GetEnvironmentVariable("MONGO_USER")}");
// Console.WriteLine($"MONGO_PASSWORD: {Environment.GetEnvironmentVariable("MONGO_PASSWORD")}");
// Console.WriteLine($"MONGO_HOST: {Environment.GetEnvironmentVariable("MONGO_HOST")}");
// Console.WriteLine($"MONGO_PORT: {Environment.GetEnvironmentVariable("MONGO_PORT")}");
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
builder.Services.AddHttpClient<UsersMicroserviceClient>(clinet => {
    clinet.BaseAddress = new Uri($"http://{Environment.GetEnvironmentVariable("USERS_MICROSERVICE_HOST")}:{Environment.GetEnvironmentVariable("USERS_MICROSERVICE_PORT")}");
});
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
