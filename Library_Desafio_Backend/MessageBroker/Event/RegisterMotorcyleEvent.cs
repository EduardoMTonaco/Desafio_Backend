using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Desafio_Backend.MessageBroker.Event
{
    public class RegisterMotorcyleEvent
    {
        public string Identificador { get; set; } 
        public int Ano { get; set; }
        public string Modelo { get; set; }
        public string Placa { get; set; }
    }
}
