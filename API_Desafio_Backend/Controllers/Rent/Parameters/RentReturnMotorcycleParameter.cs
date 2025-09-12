using System.ComponentModel;

namespace API_Desafio_Backend.Controllers.Rent.Parameters
{
    public class RentReturnMotorcycleParameter
    {
        [DefaultValue("2025-01-01T00:00:00Z")]
        public DateTime Data_Termino { get; set; }
    }
}
