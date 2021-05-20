using Microsoft.Data.Sqlite;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using System;
using System.Collections.Generic;

namespace NoleggioAutomezzi.Repository
{
    public class PrenotazioniRepository
    {
        private DatabaseConnectionRepository _repoDatabase;
        private MailRepository _repoMail;
        private UtentiRepository _repoUtenti;
        private AutomezziRepository _repoAutomezzi;
        public PrenotazioniRepository()
        {
            _repoDatabase = new DatabaseConnectionRepository();
            _repoMail = new MailRepository();
            _repoUtenti = new UtentiRepository();
            _repoAutomezzi = new AutomezziRepository();
        }

        public List<Prenotazione> ListPrenotazioni(int? idUtente)
        {
            List<Prenotazione> list = new List<Prenotazione>();
            string queryString = "SELECT * FROM Prenotazioni";
            if (idUtente.HasValue && _repoUtenti.ExistsUtenteById(idUtente.Value))
                queryString += string.Format(" WHERE IdUtente = {0}", idUtente.Value);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Prenotazione prenotazione = FillPrenotazione(reader);
                    list.Add(prenotazione);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }

        public List<Prenotazione> ListPrenotazioniDaGestire(int? idUtente)
        {
            List<Prenotazione> list = new List<Prenotazione>();
            string queryString = "SELECT * FROM Prenotazioni WHERE stato = 0";
            if (idUtente.HasValue && _repoUtenti.ExistsUtenteById(idUtente.Value))
                queryString += string.Format(" AND IdUtente = {0}", idUtente.Value);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Prenotazione prenotazione = FillPrenotazione(reader);
                    list.Add(prenotazione);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public Prenotazione GetPrenotazioneById(int id)
        {
            Prenotazione prenotazione = null;
            string queryString = string.Format("SELECT * FROM Prenotazioni WHERE Id = {0};", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    prenotazione = FillPrenotazione(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return prenotazione;
        }

        public bool InsertPrenotazione(Prenotazione prenotazione)
        {
            validatePrenotazione(prenotazione, true);

            string queryString = string.Format("INSERT INTO Prenotazioni(IdAutomezzo,IdUtente,DataInizio,DataFine,Stato) values({0},{1},'{2}','{3}', 0);",
                prenotazione.automezzo.id,
                prenotazione.utente.id,
                prenotazione.dataInizio.ToString("yyyy-MM-dd"),
                prenotazione.dataFine.ToString("yyyy-MM-dd"));

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            
            _repoDatabase.Close(connection);
            if (result)
            {
                Utente utente = _repoUtenti.GetUtenteById(prenotazione.utente.id);
                Automezzo automezzo = _repoAutomezzi.GetAutomezzoById(prenotazione.automezzo.id);
                //Invio una mail al cliente per informarlo dell'avvenuta richiesta di prenotazione.
                _repoMail.SendMail(utente.indirizzoEmail, "Richiesta di Prenotazione effettuata", string.Format(
                    "Ciao {0}," +
                    "\nAbbiamo ricevuto la tua richiesta di prenotazione per il mezzo {1} {2} dal giorno {3} al giorno {4}." +
                    "\nRiceverai una nuova mail quando la tua prenotazione verrà accettata o rifiutata.",
                    utente.nome,
                    automezzo.marca,
                    automezzo.modello,
                    prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                    prenotazione.dataFine.ToString("dd/MM/yyyy")));

                //Invio una mail anche ad admin, l'utente con id 1.
                Utente admin = _repoUtenti.GetUtenteById(1);
                _repoMail.SendMail(admin.indirizzoEmail, "Nuova Richiesta di Prenotazione", string.Format(
                    "L'utente {0} ha effettuato una richiesta di prenotazione per il mezzo {1} {2} targato {3} dal giorno {4} al giorno {5}.",
                    utente.username,
                    automezzo.marca,
                    automezzo.modello,
                    automezzo.targa,
                    prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                    prenotazione.dataFine.ToString("dd/MM/yyyy")));
            }
            else
                throw new OperationFailedException();
            return result;
        }
        public bool EditPrenotazione(Prenotazione prenotazione)
        {
            validatePrenotazione(prenotazione, false);
            Prenotazione old = GetPrenotazioneById(prenotazione.id); 
            string queryString = string.Format("UPDATE Prenotazioni SET IdAutomezzo = {0},IdUtente = {1}, DataInizio = '{2}', DataFine = '{3}' WHERE Id = {4}" ,
                prenotazione.automezzo.id,
                prenotazione.utente.id,
                prenotazione.dataInizio.ToString("yyyy-MM-dd"),
                prenotazione.dataFine.ToString("yyyy-MM-dd"),
                prenotazione.id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);

            if (result)
            {
                //TODO: mail avvenuta modifica
            }
            else
                throw new OperationFailedException();

            return result;
        }
        public bool AccettaPrenotazione(int idPrenotazione)
        {
            bool result = false;
            if (ExistsPrenotazioneById(idPrenotazione))
            {
                string queryString = string.Format("UPDATE Prenotazioni SET Stato = 1 WHERE Id = {0}", idPrenotazione);

                SqliteConnection connection = _repoDatabase.Connect();
                SqliteCommand command = new SqliteCommand(queryString, connection);

                int res = command.ExecuteNonQuery();
                result = res == 1 ? true : false;
                _repoDatabase.Close(connection);

                if (result)
                {
                    Prenotazione prenotazione = GetPrenotazioneById(idPrenotazione);
                    Utente utente = _repoUtenti.GetUtenteById(prenotazione.utente.id);
                    Automezzo automezzo = _repoAutomezzi.GetAutomezzoById(prenotazione.automezzo.id);
                    //Invio una mail al cliente per informarlo dell'accettazione della sua prenotazione.
                    _repoMail.SendMail(utente.indirizzoEmail, "Prenotazione Accettata", string.Format(
                        "Ciao {0}," +
                        "\nLa tua richiesta di prenotazione per il mezzo {1} {2} dal giorno {3} al giorno {4} " +
                        "è stata accettata.",
                        utente.nome,
                        automezzo.marca,
                        automezzo.modello,
                        prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                        prenotazione.dataFine.ToString("dd/MM/yyyy")));
                }    
                else
                    throw new OperationFailedException();
            }
            else
                throw new RecordNotFountException();

                
            return result;
        }
        public bool RifiutaPrenotazione(int idPrenotazione)
        {
            bool result = false;
            if (ExistsPrenotazioneById(idPrenotazione))
            {
                string queryString = string.Format("UPDATE Prenotazioni SET Stato = 2 WHERE Id = {0}", idPrenotazione);

                SqliteConnection connection = _repoDatabase.Connect();
                SqliteCommand command = new SqliteCommand(queryString, connection);

                int res = command.ExecuteNonQuery();
                result = res == 1 ? true : false;
                _repoDatabase.Close(connection);

                if (result)
                {
                    Prenotazione prenotazione = GetPrenotazioneById(idPrenotazione);
                    Utente utente = _repoUtenti.GetUtenteById(prenotazione.utente.id);
                    Automezzo automezzo = _repoAutomezzi.GetAutomezzoById(prenotazione.automezzo.id);
                    //Invio una mail al cliente per informarlo dell'accettazione della sua prenotazione.
                    _repoMail.SendMail(utente.indirizzoEmail, "Prenotazione Rifiutata", string.Format(
                        "Ciao {0}," +
                        "\nLa tua richiesta di prenotazione per il mezzo {1} {2} dal giorno {3} al giorno {4} " +
                        "è stata rifiutata.",
                        utente.nome,
                        automezzo.marca,
                        automezzo.modello,
                        prenotazione.dataInizio.ToString("dd/MM/yyyy"),
                        prenotazione.dataFine.ToString("dd/MM/yyyy")));
                }
                else
                    throw new OperationFailedException();
            }
            else
                throw new RecordNotFountException();


            return result;
        }
        public bool UpdatePrenotazione(Prenotazione prenotazione)
        {
            bool updated = false;
            if (ExistsPrenotazioneById(prenotazione.id))
                updated = EditPrenotazione(prenotazione);
            else
                updated = InsertPrenotazione(prenotazione);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }
        private bool validatePrenotazione(Prenotazione prenotazione, bool insertMode)
        {
            bool ok = true;
            if (prenotazione.automezzo.id <= 0)
                ok = false;
            else
                if (prenotazione.utente.id <= 0)
                    ok = false;
                else
                    if (prenotazione.dataInizio == null || prenotazione.dataInizio < DateTime.Today)
                        ok = false;
                    else
                        if (prenotazione.dataFine == null)
                            ok = false;
                        else
                            if(prenotazione.stato < 0 || prenotazione.stato > 2)
                                ok = false;


            if (insertMode)
                if (ExistsPrenotazioneById(prenotazione.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsPrenotazioneById(prenotazione.id))
                    ok = false;
            if (AutomezzoGiaPrenotato(prenotazione))
                throw new AutomezzoNonDisponibileException();
            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool AutomezzoGiaPrenotato(Prenotazione prenotazione)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Prenotazioni WHERE IdAutomezzo = {0} AND (DataInizio >= '{1}' OR dataFine <= '{2}') AND Id != {3};",
                prenotazione.utente.id, 
                prenotazione.dataInizio, 
                prenotazione.dataFine, 
                prenotazione.id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    found = true;
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return found;
        }
        private bool ExistsPrenotazioneById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Prenotazioni WHERE Id = {0};", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    found = true;
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return found;
        }
        private Prenotazione FillPrenotazione(SqliteDataReader reader)
        {
            Prenotazione prenotazione = new Prenotazione();
            prenotazione.id = int.Parse(reader["Id"].ToString());
            prenotazione.automezzo = _repoAutomezzi.GetAutomezzoById(int.Parse(reader["IdAutomezzo"].ToString()));
            prenotazione.utente = _repoUtenti.GetUtenteById(int.Parse(reader["IdUtente"].ToString()));
            prenotazione.dataInizio = DateTime.Parse(reader["DataInizio"].ToString());
            prenotazione.dataFine = DateTime.Parse(reader["DataFine"].ToString());
            prenotazione.stato = int.Parse(reader["Stato"].ToString());

            return prenotazione;
        }
    }
}
