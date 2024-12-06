using DotNetEnv;
using Ecommerce.OrderService.API.Middlewares;
using Ecommerce.OrderService.BusinessLogicLayer;
using Ecommerce.OrderService.DataLayer;
using FluentValidation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
Env.Load("./.env");
builder.Services.AddControllers();
builder.Services.AddDataLayer(builder.Configuration);
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
