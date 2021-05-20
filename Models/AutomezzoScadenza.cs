﻿using System;

namespace NoleggioAutomezzi.Models
{
    public class AutomezzoScadenza
    {
        public int id { get; set; }
        public Automezzo sutomezzo { get; set; }
        public Scadenza scadenza { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime? dataFine { get; set; }
        public int? kmIniziali { get; set; }
        public DateTime? dataPagamento { get; set; }
    }
}
