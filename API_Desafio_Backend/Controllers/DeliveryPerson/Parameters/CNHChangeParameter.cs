using System.ComponentModel;

namespace API_Desafio_Backend.Controllers.DeliveryPerson.Parameters
{
    public class CNHChangeParameter
    {
        [DefaultValue("base64string")]
        public string Imagem_Cnh { get; set; }
    }
}
