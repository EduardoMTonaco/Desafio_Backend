using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.DataBase.MongoDB.DataBase.Class
{
    public class RentClass
    {
        [BsonId]
        public string Identificador { get; set; }
        public string Entregador_id { get; set; }
        public string Moto_id { get; set; }
        public DateTime Data_inicio { get; set; }
        public DateTime? Data_termino { get; set; }
        public DateTime Data_previsao_termino { get; set; }
        public int Plano { get; set; }
        public decimal? valor { get; set; }
    }
}
