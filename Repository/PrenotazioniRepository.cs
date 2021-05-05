using Microsoft.Data.Sqlite;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Models.Full;
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

        public List<PrenotazioneFull> ListPrenotazioniFull(int? idUtente)
        {
            List<PrenotazioneFull> list = new List<PrenotazioneFull>();
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
                    PrenotazioneFull prenotazione = FillPrenotazioneFull(reader);
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
        public PrenotazioneFull GetPrenotazioneFullById(int id)
        {
            PrenotazioneFull prenotazione = null;
            string queryString = string.Format("SELECT * FROM Prenotazioni WHERE Id = {0};", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    prenotazione = FillPrenotazioneFull(reader);
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

            string queryString = string.Format("INSERT INTO Prenotazioni(IdAutomezzo,IdUtente,DataInizio,DataFine,Confermata) values({0},{1},'{2}','{3}', 0);",
                prenotazione.idAutomezzo,
                prenotazione.idUtente,
                prenotazione.dataInizio.ToString("yyyy-MM-dd"),
                prenotazione.dataFine.ToString("yyyy-MM-dd"));

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);

            return result;
        }
        public bool EditPrenotazione(Prenotazione prenotazione)
        {
            validatePrenotazione(prenotazione, false);

            string queryString = string.Format("UPDATE Prenotazioni SET IdAutomezzo = {0},IdUtente = {1}, DataInizio = '{2}', DataFine = '{3}', Confermata= {4} WHERE Id = {5}" ,
                prenotazione.idAutomezzo,
                prenotazione.idUtente,
                prenotazione.dataInizio.ToString("yyyy-MM-dd"),
                prenotazione.dataFine.ToString("yyyy-MM-dd"),
                prenotazione.id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);

            return result;
        }

        public bool UpdatePrenotazione(Prenotazione prenotazione)
        {
            bool updated = false;
            if (ExistsPrenotazioneById(prenotazione.id))
                updated = EditPrenotazione(prenotazione);
            else
                updated = InsertPrenotazione(prenotazione);

            return updated;
        }
        private bool validatePrenotazione(Prenotazione prenotazione, bool insertMode)
        {
            bool ok = true;
            if (prenotazione.idAutomezzo <= 0)
                ok = false;
            else
                if (prenotazione.idUtente <= 0)
                    ok = false;
                else
                    if (prenotazione.dataInizio == null || prenotazione.dataInizio < DateTime.Today)
                        ok = false;
                    else
                        if (prenotazione.dataFine == null)
                            ok = false;


            if (insertMode)
                if (AutomezzoGiaPrenotato(prenotazione) || ExistsPrenotazioneById(prenotazione.id))
                    ok = false;
            if (!insertMode)
                if (AutomezzoGiaPrenotato(prenotazione) || !ExistsPrenotazioneById(prenotazione.id))
                    ok = false;
            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool AutomezzoGiaPrenotato(Prenotazione prenotazione)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Prenotazioni WHERE IdAutomezzo = {0} AND (DataInizio >= {1} OR dataFine <= {2}) AND Id != {3};",
                prenotazione.idAutomezzo, 
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
            prenotazione.idAutomezzo = int.Parse(reader["IdAutomezzo"].ToString());
            prenotazione.idUtente = int.Parse(reader["IdUtente"].ToString());
            prenotazione.dataInizio = DateTime.Parse(reader["DataInizio"].ToString());
            prenotazione.dataFine = DateTime.Parse(reader["DataFine"].ToString());
            prenotazione.confermata = (reader["Confermata"].ToString() == "1");

            return prenotazione;
        }
        private PrenotazioneFull FillPrenotazioneFull(SqliteDataReader reader)
        {
            PrenotazioneFull prenotazione = new PrenotazioneFull();
            prenotazione.id = int.Parse(reader["Id"].ToString());
            prenotazione.automezzo = _repoAutomezzi.GetAutomezzoById(int.Parse(reader["IdAutomezzo"].ToString()));
            prenotazione.utente = _repoUtenti.GetUtenteById(int.Parse(reader["IdUtente"].ToString()));
            prenotazione.dataInizio = DateTime.Parse(reader["DataInizio"].ToString());
            prenotazione.dataFine = DateTime.Parse(reader["DataFine"].ToString());
            prenotazione.confermata = (reader["Confermata"].ToString() == "1");

            return prenotazione;
        }
    }
}
