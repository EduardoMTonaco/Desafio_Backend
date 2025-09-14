using MassTransit;
using MessageBroker_Desafio_Backend.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

string host = Environment.GetEnvironmentVariable("RabbitMq__Host");
string username = Environment.GetEnvironmentVariable("RabbitMq__Username");
string password = Environment.GetEnvironmentVariable("RabbitMq__Password");
string vhost = Environment.GetEnvironmentVariable("RabbitMq__VirtualHost");
string errorMessage = "";
StringBuilder sb = new StringBuilder(errorMessage);
try
{


    var factory = new ConnectionFactory()
    {
        HostName = host,
        UserName = username,
        Password = password,
        VirtualHost = vhost
    };

    int retries = 0;
    int maxRetries = 10;
    TimeSpan delay = TimeSpan.FromSeconds(5);

    while (retries < maxRetries)
    {
        try
        {

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ReceiveEndpoint("register-motorcyle_queue", e =>
                {
                    e.PrefetchCount = 1;
                    e.Durable = true;
                    e.AutoDelete = false;
                    e.Consumer<RegisterMotorcyleConsumer>();
                });
            });

            await busControl.StartAsync();

            await Task.Delay(Timeout.Infinite);


        }
        catch (BrokerUnreachableException)
        {
            throw;

        }
        catch (Exception)
        {
            throw;
        }
    }

    if (retries >= maxRetries)
    {
        throw new Exception("Unable to connect to RabbitMQ after multiple attempts.");
    }
}
catch (Exception)
{

    throw;
}
throw new Exception(sb.ToString());