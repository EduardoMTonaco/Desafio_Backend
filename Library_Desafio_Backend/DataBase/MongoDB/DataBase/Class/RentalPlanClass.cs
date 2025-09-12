using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class
{
    public class RentalPlanClass
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Identificador { get; set; }
        public int Plano { get; set; }
        public int Dias { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal Multa { get; set; }
        public decimal ValorDiariaAposTermino { get; set; }


    }
}
