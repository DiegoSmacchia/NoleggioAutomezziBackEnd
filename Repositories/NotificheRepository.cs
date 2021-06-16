using Microsoft.Data.Sqlite;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repository;
using System.Collections.Generic;
using System.Linq;

namespace NoleggioAutomezzi.Repositories
{
    public class NotificheRepository
    {
        private UtentiRepository _repoUtenti;
        private AutomezziRepository _repoAutomezzi;
        private PrenotazioniRepository _repoPrenotazioni;
        private InterventiRepository _repoInterventi;
        private MulteRepository _repoMulte;

        public NotificheRepository()
        {
            _repoUtenti = new UtentiRepository();
            _repoAutomezzi = new AutomezziRepository();
            _repoPrenotazioni = new PrenotazioniRepository();
            _repoInterventi = new InterventiRepository();
            _repoMulte = new MulteRepository();
        }

        public List<string> ListNotifiche(int idUtente)
        {
            List<string> notifiche = new List<string>();
            Utente utente = _repoUtenti.GetUtenteById(idUtente);
            if (utente.hasPermessi)
            {
                List<Prenotazione> prenotazioniDaGestire = _repoPrenotazioni.ListPrenotazioniDaGestire(null);
                foreach(Prenotazione prenotazione in prenotazioniDaGestire)
                {
                    string messaggio = string.Format("La prenotazione dell'utente {0} per l'automezzo {1} {2} targato {3}" +
                        " dal {4} al {5} è in attesa di approvazione.",
                        prenotazione.utente.username, 
                        prenotazione.automezzo.marca, 
                        prenotazione.automezzo.modello, 
                        prenotazione.automezzo.targa, 
                        prenotazione.dataInizio.ToString("dd/MM/yyyy"), 
                        prenotazione.dataFine.ToString("dd/MM/yyyy"));
                    notifiche.Add(messaggio);
                }
            }
            else
            {
                List<Prenotazione> prenotazioni = _repoPrenotazioni.ListPrenotazioni(utente.id);
                prenotazioni = prenotazioni.OrderBy(prenotazione => prenotazione.stato).ToList();
                foreach (Prenotazione prenotazione in prenotazioni)
                {
                    if(prenotazione.stato == 0)
                    {
                        string messaggio = string.Format("La tua prenotazione per l'automezzo {0} {1} targato {2}" +
                        " dal {3} al {4} è in attesa di approvazione.",
                        prenotazione.automezzo.marca,
                        prenotazione.automezzo.modello,
                        prenotazione.automezzo.targa,
                        prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                        prenotazione.dataFine.ToString("dd/MM/yyyy"));
                        notifiche.Add(messaggio);
                    }
                    if (prenotazione.stato == 1)
                    {
                        string messaggio = string.Format("La tua prenotazione per l'automezzo {0} {1} targato {2}" +
                        " dal {3} al {4} è stata accettata.",
                        prenotazione.automezzo.marca,
                        prenotazione.automezzo.modello,
                        prenotazione.automezzo.targa,
                        prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                        prenotazione.dataFine.ToString("dd/MM/yyyy"));
                        notifiche.Add(messaggio);

                    }
                    if (prenotazione.stato == 2)
                    {
                        string messaggio = string.Format("La tua prenotazione per l'automezzo {0} {1} targato {2}" +
                        " dal {3} al {4} è stata rifiutata.",
                        prenotazione.automezzo.marca,
                        prenotazione.automezzo.modello,
                        prenotazione.automezzo.targa,
                        prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                        prenotazione.dataFine.ToString("dd/MM/yyyy"));
                        notifiche.Add(messaggio);

                    }
                    
                    
                }
            }
            return notifiche;
        }

        public List<int> ListValoriAdmin()
        {
            List<int> list = new List<int>();

            int numPrenotazioni = _repoPrenotazioni.ListPrenotazioniDaGestire(null).Count;
            int numInterventi = _repoInterventi.ListInterventiDaChiudere().Count;

            list.Add(numPrenotazioni);
            list.Add(numInterventi);

            return list;
        }

        public List<int> ListValoriUtente(int idUtente)
        {
            List<int> list = new List<int>();

            int numPrenotazioni = _repoPrenotazioni.ListPrenotazioniDaGestire(idUtente).Count;
            int numGuasti = _repoInterventi.ListGuastiByIdUtente(idUtente).Count;
            int numMulte = _repoMulte.ListMulte(idUtente).Count;

            list.Add(numPrenotazioni);
            list.Add(numGuasti);
            list.Add(numMulte);

            return list;
        }
    }
}
