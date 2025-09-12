using Library_Desafio_Backend.MessageBroker.Event;
using MassTransit;

namespace MessageBroker_Desafio_Backend.Consumer
{
    public class RegisterMotorcyleConsumer : IConsumer<RegisterMotorcyleEvent>
    {
        public Task Consume(ConsumeContext<RegisterMotorcyleEvent> context)
        {
            var evento = context.Message;
            Console.WriteLine($"Moto cadastrada: {evento.Placa}, id: {evento.Identificador}");
            return Task.CompletedTask;
        }
    }
}
