using System;

namespace NoleggioAutomezzi.Models
{
    public class Intervento
    {
        public int id { get; set; }
        public Automezzo automezzo { get; set; }
        public Meccanico meccanico { get; set; }
        public Guasto guasto { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime? dataFine { get; set; }

        public Intervento()
        {
            this.automezzo = new Automezzo();
            this.meccanico = new Meccanico();
            this.guasto = new Guasto();
        }
    }

}
