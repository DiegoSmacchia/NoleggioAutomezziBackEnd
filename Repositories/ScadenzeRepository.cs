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
    public class ScadenzeRepository
    {
        AutomezziRepository _repoAutomezzi;
        DatabaseConnectionRepository _repoDatabase;

        public ScadenzeRepository()
        {
            _repoAutomezzi = new AutomezziRepository();
            _repoDatabase = new DatabaseConnectionRepository();
        }

        //Scadenza
        public List<Scadenza> ListScadenze()
        {
            List<Scadenza> list = new List<Scadenza>();

            string queryString = "SELECT * FROM Scadenze";
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Scadenza scadenza = FillScadenza(reader);
                    list.Add(scadenza);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public Scadenza GetScadenzaById(int id)
        {
            Scadenza scadenza = null;

            string queryString = string.Format("SELECT * FROM Scadenze WHERE Id = {0}", id);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    scadenza = FillScadenza(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return scadenza;
        }
        public bool InsertScadenza(Scadenza scadenza)
        {
            bool result = false;
            ValidateScadenza(scadenza, true);

            string queryString = string.Format("INSERT INTO Scadenze(Scadenza) values('{0}');",
                scadenza.scadenza);

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

            if(!result)
                throw new OperationFailedException();
            return result;
        }
        public bool EditScadenza(Scadenza scadenza)
        {
            bool result = false;
            ValidateScadenza(scadenza, false);
            Scadenza old = GetScadenzaById(scadenza.id);
            string queryString = string.Format("UPDATE Scadenze SET Scadenza = '{0}' WHERE Id = {1}",
                scadenza.scadenza,
                scadenza.id);

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
        public bool UpdateScadenza(Scadenza scadenza)
        {
            bool updated = false;
            if (ExistsScadenzaById(scadenza.id))
                updated = EditScadenza(scadenza);
            else
                updated = InsertScadenza(scadenza);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }
        private bool ValidateScadenza(Scadenza scadenza, bool insertMode)
        {
            bool ok = true;
            if (scadenza.scadenza == null || scadenza.scadenza == "")
                ok = false;

            if (insertMode)
                if (ExistsScadenzaById(scadenza.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsScadenzaById(scadenza.id))
                    ok = false;
            
            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool ExistsScadenzaById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Scadenze WHERE Id = {0};", id);

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
        private Scadenza FillScadenza(SqliteDataReader reader)
        {
            Scadenza scadenza = new Scadenza();
            scadenza.id = int.Parse(reader["Id"].ToString());
            scadenza.scadenza = reader["Scadenza"].ToString();

            return scadenza;
        }

        //AutomezzoScadenza
        public List<AutomezzoScadenza> ListAutomezzoScadenze(int? idAutomezzo)
        {
            List<AutomezzoScadenza> list = new List<AutomezzoScadenza>();

            string queryString = "SELECT * FROM AutomezziScadenze";
            if (idAutomezzo.HasValue)
                queryString += string.Format(" WHERE IdAutomezzo = {0}", idAutomezzo.Value);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    AutomezzoScadenza scadenza = FillAutomezzoScadenza(reader);
                    list.Add(scadenza);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public AutomezzoScadenza GetAutomezzoScadenzaById(int id)
        {
            AutomezzoScadenza scadenza = null;

            string queryString = string.Format("SELECT * FROM AutomezziScadenze WHERE Id = {0}", id);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    scadenza = FillAutomezzoScadenza(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return scadenza;
        }
        public bool InsertAutomezzoScadenza(AutomezzoScadenza scadenza)
        {
            bool result = false;
            ValidateAutomezzoScadenza(scadenza, true);

            string queryString = string.Format("INSERT INTO AutomezziScadenze(IdAutomezzo,IdScadenza,DataInizio,DataFine,KmIniziali,DataPagamento) " +
                "values({0}, {1}, '{2}', {3}, {4}, {5});",
                scadenza.automezzo.id, 
                scadenza.scadenza.id, 
                scadenza.dataInizio.ToString("yyyy-MM-dd"), 
                scadenza.dataFine.HasValue ? "'" + scadenza.dataFine.Value.ToString("yyyy-MM-dd") + "'" : "null",
                scadenza.dataPagamento.HasValue ? "'" + scadenza.dataPagamento.Value.ToString("yyyy-MM-dd") + "'" : "null");

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
        public bool EditAutomezzoScadenza(AutomezzoScadenza scadenza)
        {
            bool result = false;
            ValidateAutomezzoScadenza(scadenza, false);
            AutomezzoScadenza old = GetAutomezzoScadenzaById(scadenza.id);
            string queryString = string.Format("EDIT AutomezziScadenze SET " +
                "IdAutomezzo = {0}," +
                "IdScadenza = {1}," +
                "DataInizio = '{2}'," +
                "DataFine = {3}," +
                "KmIniziali = {4}," +
                "DataPagamento = {5};",
                scadenza.automezzo.id,
                scadenza.scadenza.id,
                scadenza.dataInizio.ToString("yyyy-MM-dd"),
                scadenza.dataFine.HasValue ? "'" + scadenza.dataFine.Value.ToString("yyyy-MM-dd") + "'" : "null",
                scadenza.dataPagamento.HasValue ? "'" + scadenza.dataPagamento.Value.ToString("yyyy-MM-dd") + "'" : "null");

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
        public bool UpdateAutomezzoScadenza(AutomezzoScadenza scadenza)
        {
            bool updated = false;
            if (ExistsAutomezzoScadenzaById(scadenza.id))
                updated = EditAutomezzoScadenza(scadenza);
            else
                updated = InsertAutomezzoScadenza(scadenza);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }
        private bool ValidateAutomezzoScadenza(AutomezzoScadenza scadenza, bool insertMode)
        {
            bool ok = true;
            if ( !ExistsScadenzaById(scadenza.scadenza.id) )
                ok = false;
            if ( !_repoAutomezzi.ExistsAutomezzoById(scadenza.automezzo.id) )
                ok = false;
            if (scadenza.dataInizio == null)
                ok = false;
            if (scadenza.kmIniziali < 0)
                ok = false;

            if (insertMode)
                if (ExistsAutomezzoScadenzaById(scadenza.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsAutomezzoScadenzaById(scadenza.id))
                    ok = false;

            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool ExistsAutomezzoScadenzaById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM AutomezziScadenze WHERE Id = {0};", id);

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
        private AutomezzoScadenza FillAutomezzoScadenza(SqliteDataReader reader)
        {
            AutomezzoScadenza scadenza = new AutomezzoScadenza();
            scadenza.id = int.Parse(reader["Id"].ToString());
            scadenza.scadenza = GetScadenzaById(int.Parse(reader["IdScadenza"].ToString()));
            scadenza.automezzo = _repoAutomezzi.GetAutomezzoById(int.Parse(reader["IdAutomezzo"].ToString()));
            scadenza.dataInizio = DateTime.Parse(reader["DataInizio"].ToString());
            scadenza.dataFine = DateTime.Parse(reader["DataFine"].ToString());
            scadenza.kmIniziali = int.Parse(reader["KmIniziali"].ToString());
            scadenza.dataPagamento = DateTime.Parse(reader["DataPagamento"].ToString());

            return scadenza;
        }
    }
}
