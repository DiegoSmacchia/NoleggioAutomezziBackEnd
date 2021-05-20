using System;

namespace NoleggioAutomezzi.Models
{
    public class Intervento
    {
        public int id { get; set; }
        public Automezzo automezzo { get; set; }
        public Meccanico meccanico { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime? dataFine { get; set; }
    }
}
