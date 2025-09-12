using Library_Desafio_Backend.MessageBroker.Event;
using MassTransit;
using MessageBroker_Desafio_Backend.Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory()) // Pega o diretório da aplicação
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

var rabbitMqConfig = configuration.GetSection("RabbitMq");
string host = rabbitMqConfig["Host"];
string username = rabbitMqConfig["Username"];
string password = rabbitMqConfig["Password"];
string vhost = rabbitMqConfig["VirtualHost"];

// Criar o bus
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    if (string.IsNullOrEmpty(vhost) || vhost == "/")
    {
        // Se não usar virtual host ou for default "/", não precisa passar
        cfg.Host(host, h =>
        {
            h.Username(username);
            h.Password(password);
        });
    }
    else
    {
        // Usar virtual host customizado
        cfg.Host(host, vhost, h =>
        {
            h.Username(username);
            h.Password(password);
        });
    }

    // Definir a fila que vai receber as mensagens e associar o consumidor
    cfg.ReceiveEndpoint("produto-cadastrado-queue", e =>
    {
        e.Consumer<RegisterMotorcyleConsumer>();
    });
});

await busControl.StartAsync();

Console.WriteLine("Aguardando eventos. Pressione qualquer tecla para sair.");
Console.ReadKey();

await busControl.StopAsync();