using Library_Desafio_Backend.MessageBroker.Event;
using MassTransit;

namespace MessageBroker_Desafio_Backend.Consumer
{
    public class RegisterMotorcyleConsumer : IConsumer<RegisterMotorcyleEvent>
    {
        public async Task Consume(ConsumeContext<RegisterMotorcyleEvent> context)
        {
            RegisterMotorcyleEvent motorcycleEvent = context.Message;
            if(motorcycleEvent.Ano == 2024)
            {
                Console.WriteLine($"Motorcycle={motorcycleEvent.Identificador}, Year={motorcycleEvent.Ano}, Model={motorcycleEvent.Modelo}, Plate={motorcycleEvent.Placa}");
            }
           
        }
    }
}
