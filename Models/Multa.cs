using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Models
{
    public class Multa
    {
        public int id { get; set; }
        public Prenotazione prenotazione { get; set; }
        public decimal importo { get; set; }
        public DateTime data { get; set; }
    }
}
