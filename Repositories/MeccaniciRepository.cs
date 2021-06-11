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
    public class MeccaniciRepository
    {
        DatabaseConnectionRepository _repoDatabase;

        public MeccaniciRepository()
        {
            _repoDatabase = new DatabaseConnectionRepository();
        }

        //Meccanico
        public List<Meccanico> ListMeccanici()
        {
            List<Meccanico> list = new List<Meccanico>();

            string queryString = "SELECT * FROM Meccanici";
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Meccanico meccanico = FillMeccanico(reader);
                    list.Add(meccanico);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public Meccanico GetMeccanicoById(int id)
        {
            Meccanico meccanico = null;

            string queryString = string.Format("SELECT * FROM Meccanici WHERE Id = {0}", id);
            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    meccanico = FillMeccanico(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return meccanico;
        }
        public bool InsertMeccanico(Meccanico meccanico)
        {
            bool result = false;
            ValidateMeccanico(meccanico, true);

            string queryString = string.Format("INSERT INTO Meccanici(RagioneSociale, Indirizzo, Telefono) values('{0}', '{1}', '{2}');",
                meccanico.ragioneSociale, meccanico.indirizzo, meccanico.telefono);

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
        public bool EditMeccanico(Meccanico meccanico)
        {
            bool result = false;
            ValidateMeccanico(meccanico, false);
            Meccanico old = GetMeccanicoById(meccanico.id);
            string queryString = string.Format("UPDATE Meccanici SET RagioneSociale = '{0}', Indirizzo = '{1}', Telefono = '{2}' WHERE Id = {3}",
                meccanico.ragioneSociale,
                meccanico.indirizzo,
                meccanico.telefono,
                meccanico.id);

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
        public bool UpdateMeccanico(Meccanico meccanico)
        {
            bool updated;
            if (ExistsMeccanicoById(meccanico.id))
                updated = EditMeccanico(meccanico);
            else
                updated = InsertMeccanico(meccanico);

            if (!updated)
                throw new OperationFailedException();

            return updated;
        }

        public bool DeleteMeccanico(int id)
        {
            bool result = false;
            if (CanDeleteMeccanico(id))
            {
                string queryString = string.Format("DELETE FROM Meccanici WHERE Id = {0};", id);

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
        private bool ValidateMeccanico(Meccanico meccanico, bool insertMode)
        {
            bool ok = true;
            if (meccanico.ragioneSociale == null || meccanico.ragioneSociale == "")
                ok = false;

            if (meccanico.indirizzo == null || meccanico.indirizzo == "")
                ok = false;

            if (meccanico.telefono == null || meccanico.telefono == "")
                ok = false;

            if (insertMode)
                if (ExistsMeccanicoById(meccanico.id))
                    ok = false;
            if (!insertMode)
                if (!ExistsMeccanicoById(meccanico.id))
                    ok = false;

            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool CanDeleteMeccanico(int id)
        {
            bool result = false;

            string queryString = string.Format("SELECT * FROM Interventi WHERE IdMeccanico = {0}", id);

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
        private bool ExistsMeccanicoById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Meccanici WHERE Id = {0};", id);

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
        private Meccanico FillMeccanico(SqliteDataReader reader)
        {
            Meccanico meccanico = new Meccanico();
            meccanico.id = int.Parse(reader["Id"].ToString());
            meccanico.ragioneSociale = reader["RagioneSociale"].ToString();
            meccanico.indirizzo = reader["Indirizzo"].ToString();
            meccanico.telefono = reader["Telefono"].ToString();

            return meccanico;
        }

    }
}
