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
    public class InterventiRepository
    {
        DatabaseConnectionRepository _repoDatabase;
        AutomezziRepository _repoAutomezzi;
        MeccaniciRepository _repoMeccanici;
        UtentiRepository _repoUtenti;
        MailRepository _repoMail;

        public InterventiRepository()
        {
            _repoDatabase = new DatabaseConnectionRepository();
            _repoAutomezzi = new AutomezziRepository();
            _repoMeccanici = new MeccaniciRepository();
            _repoUtenti = new UtentiRepository();
            _repoMail = new MailRepository();
        }

        //Intervento
        public List<Intervento> ListInterventi()
        {
            List<Intervento> list = new List<Intervento>();

            string queryString = "SELECT * FROM Interventi";
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Intervento intervento = FillIntervento(reader);
                    list.Add(intervento);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public List<Intervento> ListInterventiByIdAutomezzo(int idAutomezzo)
        {
            List<Intervento> list = new List<Intervento>();

            string queryString = string.Format("SELECT * FROM Interventi WHERE IdAutomezzo = {0}", idAutomezzo);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Intervento intervento = FillIntervento(reader);
                    list.Add(intervento);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public List<Intervento> ListInterventiDaChiudere()
        {
            List<Intervento> list = new List<Intervento>();
            string queryString = "SELECT * FROM Interventi WHERE dataFine IS NULL";

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Intervento intervento = FillIntervento(reader);
                    list.Add(intervento);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public Intervento GetInterventoById(int id)
        {
            Intervento intervento = null;

            string queryString = string.Format("SELECT * FROM Interventi WHERE Id = {0}", id);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    intervento = FillIntervento(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return intervento;
        }
        public bool InsertIntervento(Intervento intervento)
        {
            bool result = false;
            ValidateIntervento(intervento, true);

            string queryString = string.Format("INSERT INTO Interventi(IdAutomezzo, IdMeccanico, IdGuasto, DataInizio, DataFine) values({0}, {1}, {2}, '{3}', {4});",
                intervento.automezzo.id,
                intervento.meccanico.id,
                intervento.guasto != null && intervento.guasto.id != 0 ? intervento.guasto.id.ToString() : "null",
                intervento.dataInizio.ToString("yyyy-MM-dd"),
                intervento.dataFine.HasValue ? "'" + intervento.dataFine.Value.ToString("yyyy-MM-dd") + "'" : "null");

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
        public bool EditIntervento(Intervento intervento)
        {
            bool result = false;
            ValidateIntervento(intervento, false);
            Intervento old = GetInterventoById(intervento.id);
            string queryString = string.Format("UPDATE Interventi SET IdAutomezzo = {0}, IdMeccanico = {1}, IdGuasto = {2}, DataInizio = '{3}', DataFine = {4} WHERE Id = {5}",
                intervento.automezzo.id,
                intervento.meccanico.id,
                intervento.guasto != null && intervento.guasto.id != 0 ? intervento.guasto.id.ToString() : "null",
                intervento.dataInizio.ToString("yyyy-MM-dd"),
                intervento.dataFine.HasValue ? "'" + intervento.dataFine.Value.ToString("yyyy-MM-dd") + "'" : "null",
                intervento.id);

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
        public bool UpdateIntervento(Intervento intervento)
        {
            bool updated;
            if (ExistsInterventoById(intervento.id))
                updated = EditIntervento(intervento);
            else
                updated = InsertIntervento(intervento);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }
        public bool DeleteIntervento(int id)
        {
            bool result = false;
            string queryString = string.Format("DELETE FROM Interventi WHERE Id = {0};", id);

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
        
        private bool ValidateIntervento(Intervento intervento, bool insertMode)
        {
            bool ok = true;
            if (intervento.automezzo == null || intervento.automezzo.id <= 0)
                ok = false;

            if (intervento.meccanico == null || intervento.meccanico.id <= 0)
                ok = false;

            if (intervento.dataInizio == null)
                ok = false;

            if (insertMode)
                if (ExistsInterventoById(intervento.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsInterventoById(intervento.id))
                    ok = false;

            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool ExistsInterventoById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Interventi WHERE Id = {0};", id);

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
        private Intervento FillIntervento(SqliteDataReader reader)
        {
            DateTime datafine;
            int idguasto;
            Intervento intervento = new Intervento();
            intervento.id = int.Parse(reader["Id"].ToString());
            intervento.automezzo = _repoAutomezzi.GetAutomezzoById(int.Parse(reader["IdAutomezzo"].ToString()));
            intervento.meccanico = _repoMeccanici.GetMeccanicoById(int.Parse(reader["IdMeccanico"].ToString()));
            intervento.dataInizio = DateTime.Parse(reader["DataInizio"].ToString());
            if (DateTime.TryParse(reader["DataFine"].ToString(), out datafine))
                intervento.dataFine = datafine;
            else
                intervento.dataFine = null;

            if (int.TryParse(reader["IdGuasto"].ToString(), out idguasto))
                intervento.guasto = GetGuastoById(idguasto);
            else
                intervento.guasto = null;

            return intervento;
        }

        //Guasto
        public List<Guasto> ListGuasti()
        {
            List<Guasto> list = new List<Guasto>();

            string queryString = "SELECT * FROM Guasti";
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Guasto guasto = FillGuasto(reader);
                    list.Add(guasto);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public List<Guasto> ListGuastiByIdAutomezzo(int idAutomezzo)
        {
            List<Guasto> list = new List<Guasto>();

            string queryString = string.Format("SELECT * FROM Guasti WHERE IdAutomezzo = {0}", idAutomezzo);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Guasto guasto = FillGuasto(reader);
                    list.Add(guasto);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public List<Guasto> ListGuastiByIdUtente(int idUtente)
        {
            List<Guasto> list = new List<Guasto>();

            string queryString = string.Format("SELECT * FROM Guasti WHERE IdUtente = {0}", idUtente);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Guasto guasto = FillGuasto(reader);
                    list.Add(guasto);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public Guasto GetGuastoById(int id)
        {
            Guasto guasto = null;

            string queryString = string.Format("SELECT * FROM Guasti WHERE Id = {0}", id);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    guasto = FillGuasto(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return guasto;
        }
        public bool InsertGuasto(Guasto guasto)
        {
            bool result = false;
            ValidateGuasto(guasto, true);

            string queryString = string.Format("INSERT INTO Guasti(IdAutomezzo, IdUtente, Descrizione, Data) values({0}, {1}, '{2}', '{3}');",
                guasto.automezzo.id,
                guasto.utente.id,
                guasto.descrizione,
                guasto.data.ToString("yyyy-MM-dd"));

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            try
            {
                int res = command.ExecuteNonQuery();
                result = res == 1 ? true : false;
                if (result)
                {
                    //Invio una mail ad admin per avvisarlo del guasto
                    Utente admin = _repoUtenti.GetUtenteById(1);
                    Utente utente = _repoUtenti.GetUtenteById(guasto.utente.id);
                    Automezzo automezzo = _repoAutomezzi.GetAutomezzoById(guasto.automezzo.id);
                    _repoMail.SendMail(admin.indirizzoEmail, "Guasto Segnalato", string.Format(
                        "Buongiorno {0},\n" +
                        "E' stato segnalato un guasto dall'utente {1}:\n" +
                        "Automezzo: {2} {3} {4}\n" +
                        "Data: {5}\n" +
                        "Descrizione: {6}",
                        admin.username,
                        utente.username,
                        automezzo.marca,
                        automezzo.modello,
                        automezzo.targa,
                        guasto.data.ToString("dd/MM/yyyy"),
                        guasto.descrizione));
                }
            }
            finally
            {
                _repoDatabase.Close(connection);
            }

            if (!result)
                throw new OperationFailedException();
            return result;
        }
        public bool EditGuasto(Guasto guasto)
        {
            bool result = false;
            ValidateGuasto(guasto, false);
            Guasto old = GetGuastoById(guasto.id);
            string queryString = string.Format("UPDATE Guasti SET IdAutomezzo = {0}, IdUtente = {1}, Descrizione = '{2}', Data = '{3}' WHERE Id = {4}",
                guasto.automezzo.id,
                guasto.utente.id,
                guasto.descrizione,
                guasto.data.ToString("yyyy-MM-dd"),
                guasto.id);

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
        public bool UpdateGuasto(Guasto guasto)
        {
            bool updated;
            if (ExistsGuastoById(guasto.id))
                updated = EditGuasto(guasto);
            else
                updated = InsertGuasto(guasto);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }
        public bool DeleteGuasto(int id)
        {
            bool result = false;
            if (CanDeleteGuasto(id))
            {
                string queryString = string.Format("DELETE FROM Guasti WHERE Id = {0};", id);

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
            }

            return result;
        }
        private bool ValidateGuasto(Guasto guasto, bool insertMode)
        {
            bool ok = true;
            if (guasto.automezzo == null || guasto.automezzo.id <= 0)
                ok = false;

            if (guasto.utente == null || guasto.utente.id <= 0)
                ok = false;

            if (guasto.descrizione == null || guasto.descrizione == "")
                ok = false;

            if (insertMode)
                if (ExistsGuastoById(guasto.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsGuastoById(guasto.id))
                    ok = false;

            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool ExistsGuastoById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Guasti WHERE Id = {0};", id);

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
        private Guasto FillGuasto(SqliteDataReader reader)
        {
            Guasto guasto = new Guasto();
            guasto.id = int.Parse(reader["Id"].ToString());
            guasto.automezzo = _repoAutomezzi.GetAutomezzoById(int.Parse(reader["IdAutomezzo"].ToString()));
            guasto.utente = _repoUtenti.GetUtenteById(int.Parse(reader["IdUtente"].ToString()));
            guasto.descrizione = reader["Descrizione"].ToString();
            guasto.data = DateTime.Parse(reader["Data"].ToString());

            return guasto;
        }
        private bool CanDeleteGuasto(int id)
        {
            bool result = false;

            string queryString = string.Format("SELECT * FROM Interventi WHERE IdGuasto = {0}", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    result = false;
                else
                    result = true;
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return result;
        }
    }
}
