namespace Library_Desafio_Backend.MessageBroker.Event
{
    public class RegisterMotorcyleEvent
    {
        public string Identificador { get; set; }
        public int Ano { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
    }
}
