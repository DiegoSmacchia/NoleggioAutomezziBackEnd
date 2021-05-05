using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Models.Full
{
    public class PrenotazioneFull
    {
        public int id { get; set; }
        public Utente utente { get; set; }
        public Automezzo automezzo { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime dataFine { get; set; }
        public bool confermata { get; set; }
    }
}
