using Microsoft.Data.Sqlite;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using NoleggioAutomezzi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Repositories
{
    public class MulteRepository
    {
        private DatabaseConnectionRepository _repoDatabase;
        private MailRepository _repoMail;
        private UtentiRepository _repoUtenti;
        private AutomezziRepository _repoAutomezzi;
        private PrenotazioniRepository _repoPrenotazioni;
        public MulteRepository()
        {
            _repoDatabase = new DatabaseConnectionRepository();
            _repoMail = new MailRepository();
            _repoUtenti = new UtentiRepository();
            _repoAutomezzi = new AutomezziRepository();
            _repoPrenotazioni = new PrenotazioniRepository();
        }

        public List<Multa> ListMulte(int? idUtente)
        {
            List<Multa> list = new List<Multa>();
            string queryString = "SELECT * FROM Multe";
            if (idUtente.HasValue && _repoUtenti.ExistsUtenteById(idUtente.Value))
                queryString += string.Format(" WHERE IdUtente = {0}", idUtente.Value);


            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Multa multa = FillMulta(reader);
                    list.Add(multa);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }

       
        public Multa GetMultaById(int id)
        {
            Multa multa = null;
            string queryString = string.Format("SELECT * FROM Multe WHERE Id = {0};", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    multa = FillMulta(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return multa;
        }
        public bool InsertMulta(Multa multa)
        {
            bool result = false;
            ValidateMulta(multa, true);

            string queryString = string.Format("INSERT INTO Multe(IdPrenotazione, Importo, Data) values({0},{1},'{2}');",
                multa.prenotazione.id,
                multa.importo,
                multa.data.ToString("yyyy-MM-dd"));

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            try
            {
                int res = command.ExecuteNonQuery();
                result = res == 1 ? true : false;
            }
            finally
            {
                _repoDatabase.Close(connection);
            }

            if (!result)
                throw new OperationFailedException();
            return result;
        }
        public bool EditMulta(Multa multa)
        {
            bool result = false;
            ValidateMulta(multa, false);
            Multa old = GetMultaById(multa.id);
            string queryString = string.Format("UPDATE Multe SET IdPrenotazione = {0}, Importo = {1}, Data = '{2}' WHERE Id = {3}",
                multa.prenotazione.id,
                multa.importo,
                multa.data.ToString("yyyy-MM-dd"),
                multa.id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            try
            {
                int res = command.ExecuteNonQuery();
                result = res == 1 ? true : false;
            }
            finally
            {
                _repoDatabase.Close(connection);
            }

            if (!result)
                throw new OperationFailedException();

            return result;
        }
        public bool UpdateMulta(Multa multa)
        {
            bool updated = false;
            if (ExistsMultaById(multa.id))
                updated = EditMulta(multa);
            else
                updated = InsertMulta(multa);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }
        public bool DeleteMulta(int id)
        {
            string queryString = string.Format("DELETE FROM Multe WHERE Id = {0}", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            try
            {
                int res = command.ExecuteNonQuery();
            }
            finally
            {
                _repoDatabase.Close(connection);
            }
            return ExistsMultaById(id);
        }
        private bool ValidateMulta(Multa multa, bool insertMode)
        {
            bool ok = true;
            if (multa.prenotazione.id <= 0 || multa.prenotazione == null)
                ok = false;
            else
                if (multa.importo <= 0)
                ok = false;
            else
                    if (multa.data == null)
                ok = false;

            if (insertMode)
                if (ExistsMultaById(multa.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsMultaById(multa.id))
                    ok = false;
            
            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool ExistsMultaById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Multe WHERE Id = {0};", id);

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
        private Multa FillMulta(SqliteDataReader reader)
        {
            Multa multa = new Multa();
            multa.id = int.Parse(reader["Id"].ToString());
            multa.prenotazione = _repoPrenotazioni.GetPrenotazioneById(int.Parse(reader["IdPrenotazione"].ToString()));
            multa.importo = int.Parse(reader["Importo"].ToString());
            multa.data = DateTime.Parse(reader["Data"].ToString());

            return multa;
        }
    }
}
