using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Models
{
    public class Automezzo
    {
        public int id { get; set; }
        public string marca { get; set; }
        public string modello { get; set; }
        public string targa { get; set; }
        public int kmAttuali { get; set; }
        public bool mezzoDisponibile { get; set; }
    }
}
