using System.ComponentModel;

namespace API_Desafio_Backend.Controllers.Motorcycle.Parameters
{
    public class MotorcycleChangePlateParameter
    {
        [DefaultValue("ABC-1234")]
        public string Placa { get; set; }
    }
}
