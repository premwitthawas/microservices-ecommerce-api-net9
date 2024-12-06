using System.Text.Json.Serialization;
using ECommerce.Users.API.Middlewares;
using ECommerce.Users.Core;
using ECommerce.Users.Infrastructure;
using FluentValidation.AspNetCore;
using ECommerce.Users.Core.AutoMappers;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure();
builder.Services.AddCore();
builder.Services.AddControllers().AddJsonOptions(opt=>{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddAutoMapper(typeof(ApplicationUserMappingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => new { Message = "API RUNNING" });
app.UseExceptionHandlingMiddleware();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
