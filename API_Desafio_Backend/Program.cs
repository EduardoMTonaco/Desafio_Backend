using Library_Desafio_Backend.DataBase.MongoDB.DataBase;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Desafio",
        Version = "v1",
        Description = "Criação da API para o desafio"
    });
    string filePath = Path.Combine(System.AppContext.BaseDirectory, "API_Desafio_Backend.xml");
    c.IncludeXmlComments(filePath);
});
builder.Services.AddDbContext<MotorcycleRentDbContext>(options =>
{
    options.UseMongoDB(builder.Configuration["MongoDbSettings:ConnectionString"], builder.Configuration["MongoDbSettings:DatabaseName"]);
});

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {        
        string rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        string rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest";
        string rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });
    });
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Desafio v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
