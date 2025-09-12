using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class
{
    public class DeliveryPersonClass
    {
        [BsonId]
        public string Identificador { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public DateTime Data_nascimento { get; set; }
        public string Numero_Cnh { get; set; }
        public string Tipo_Cnh { get; set; }
        public string? Imagem_Cnh { get; set; }
    }
}
