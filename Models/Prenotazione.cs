using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Models
{
    public class Prenotazione
    {
        public int id { get; set; }
        public Utente utente { get; set; }
        public Automezzo automezzo { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime dataFine { get; set; }
        public int stato { get; set; }

        public Prenotazione()
        {
            this.utente = new Utente();
            this.automezzo = new Automezzo();
        }
    }
}
