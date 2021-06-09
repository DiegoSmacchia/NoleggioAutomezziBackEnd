using System;

namespace NoleggioAutomezzi.Models
{
    public class Guasto
    {
        public int id { get; set; }
        public Automezzo automezzo { get; set; }
        public Utente utente { get; set; }
        public string descrizione { get; set; }
        public DateTime data { get; set; }

        public Guasto()
        {
            this.automezzo = new Automezzo();
            this.utente = new Utente();
        }
    }
}
