using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class
{
    public class MotorcycleClass
    {
        [BsonId]
        public string Identificador { get; set; }
        public int Ano { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
    }

}
