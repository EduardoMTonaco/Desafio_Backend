using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class
{
    public class MotorcycleHistoryClass
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Identificador { get; set; }
        public string Moto_id { get; set; }
        public DateTime Data_cadastro { get; set; }
    }
}
