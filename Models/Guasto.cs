using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Models
{
    public class Guasto
    {
        public int id { get; set; }
        public Automezzo automezzo { get; set; }
        public Utente Utente { get; set; }
        public string Descrizione { get; set; }
        public DateTime data { get; set; }
    }
}
