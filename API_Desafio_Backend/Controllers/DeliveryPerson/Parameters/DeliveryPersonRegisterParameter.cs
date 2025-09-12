using System.ComponentModel;

namespace API_Desafio_Backend.Controllers.DeliveryPerson.Parameters
{
    public class DeliveryPersonRegisterParameter
    {
        [DefaultValue("Entregador Exemplo")]
        public string Identificador { get; set; }
        [DefaultValue("Nome Exemplo")]
        public string Nome { get; set; }
        [DefaultValue("48642645000154")]
        public string Cnpj { get; set; }
        [DefaultValue("1990-01-01T00:00:00Z")]
        public DateTime Data_nascimento { get; set; }
        [DefaultValue("12345678900")]
        public string Numero_Cnh { get; set; }
        [DefaultValue("AB")]
        public string Tipo_Cnh { get; set; }
        [DefaultValue("base64string")]
        public string? Imagem_Cnh { get; set; }
}
}
