using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Models
{
    public class Utente
    {
        public int id { get; set; }
        public string username { get; set; }
        public string nome { get; set; }
        public string cognome { get; set; }
        public string password { get; set; }
        public DateTime dataNascita { get; set; }
        public string indirizzoEmail { get; set; }
        public bool hasPermessi { get; set; }
    }
}
