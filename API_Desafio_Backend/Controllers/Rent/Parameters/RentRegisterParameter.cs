using System.ComponentModel;

namespace API_Desafio_Backend.Controllers.Rent.Parameters
{
    public class RentRegisterParameter
    {
        [DefaultValue("EntregadorExemplo")]
        public string Entregador_Id { get; set; }
        [DefaultValue("MotoExemplo")]
        public string Moto_Id { get; set; }
        [DefaultValue("2024-01-01T00:00:00Z")]
        public DateTime Data_Inicio { get; set; }
        [DefaultValue("2025-01-01T00:00:00Z")]
        public DateTime? Data_Termino { get; set; }
        [DefaultValue("2025-01-01T23:59:59Z")]
        public DateTime Data_Previsao_Termino { get; set; }
        [DefaultValue("1")]
        public int? Plano { get; set; }
    }
}
