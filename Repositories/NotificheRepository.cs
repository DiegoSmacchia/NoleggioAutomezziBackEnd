using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Models.Full;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Repositories
{
    public class NotificheRepository
    {
        private UtentiRepository _repoUtenti;
        private AutomezziRepository _repoAutomezzi;
        private PrenotazioniRepository _repoPrenotazioni;

        public NotificheRepository()
        {
            _repoUtenti = new UtentiRepository();
            _repoAutomezzi = new AutomezziRepository();
            _repoPrenotazioni = new PrenotazioniRepository();
        }

        public List<string> listNotifiche(int idUtente)
        {
            List<string> notifiche = new List<string>();
            Utente utente = _repoUtenti.GetUtenteById(idUtente);
            if (utente.hasPermessi)
            {
                List<PrenotazioneFull> prenotazioniDaGestire = _repoPrenotazioni.ListPrenotazioniFullDaGestire(null);
                foreach(PrenotazioneFull prenotazione in prenotazioniDaGestire)
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
                List<PrenotazioneFull> prenotazioni = _repoPrenotazioni.ListPrenotazioniFull(utente.id);
                prenotazioni = prenotazioni.OrderBy(prenotazione => prenotazione.stato).ToList();
                foreach (PrenotazioneFull prenotazione in prenotazioni)
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
    }
}
