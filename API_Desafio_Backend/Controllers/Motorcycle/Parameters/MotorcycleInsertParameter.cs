using System.ComponentModel;

namespace API_Desafio_Backend.Controllers.Motorcycle.Parameters
{
    public class MotorcycleInsertParameter
    {
        [DefaultValue("Moto Exemplo")]
        public string Identificador { get; set; } 
        [DefaultValue(2025)]
        public int Ano { get; set; }
        [DefaultValue("Modelo da Moto")]
        public string Modelo { get; set; }       
        [DefaultValue("ABC-1234")]
        public string Placa { get; set; } 
    }
}
