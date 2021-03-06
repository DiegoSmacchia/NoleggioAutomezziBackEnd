using Microsoft.Data.Sqlite;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using System.Collections.Generic;

namespace NoleggioAutomezzi.Repository
{
    public class AutomezziRepository
    {
        private DatabaseConnectionRepository _repoDatabase;
        private MailRepository _repoMail;
        public AutomezziRepository()
        {
            _repoDatabase = new DatabaseConnectionRepository();
            _repoMail = new MailRepository();
        }

        public List<Automezzo> ListAutomezzi()
        {
            List<Automezzo> list = new List<Automezzo>();
            string queryString = "SELECT * FROM Automezzi;";

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);
            
            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Automezzo automezzo = fillAutomezzo(reader);
                    list.Add(automezzo);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public List<Automezzo> ListAutomezziDisponibili()
        {
            List<Automezzo> list = new List<Automezzo>();
            string queryString = "SELECT * FROM Automezzi WHERE MezzoDisponibile = 1;";

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Automezzo automezzo = fillAutomezzo(reader);
                    list.Add(automezzo);
                }
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return list;
        }
        public Automezzo GetAutomezzoById(int Id)
        {
            Automezzo automezzo = null;
            string queryString = string.Format("SELECT * FROM Automezzi WHERE Id = {0};", Id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    automezzo = fillAutomezzo(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return automezzo;
        }

        public bool InsertAutomezzo(Automezzo automezzo)
        {
            validateAutomezzo(automezzo, true);
            
            string queryString = string.Format("INSERT INTO Automezzi(Targa, Marca,Modello, KmAttuali, MezzoDisponibile) values('{0}','{1}','{2}',{3}, 1);",
                automezzo.targa,
                automezzo.marca.Replace("'", "''"),
                automezzo.modello.Replace("'", "''"),
                automezzo.kmAttuali);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);

            return result;
        }
        public bool EditAutomezzo(Automezzo automezzo)
        {
            validateAutomezzo(automezzo, false);

            string queryString = string.Format("UPDATE Automezzi SET Targa = '{0}', Marca = '{1}',Modello = '{2}', KmAttuali = {3}, MezzoDisponibile = {4} WHERE Id = {5};",
                automezzo.targa,
                automezzo.marca.Replace("'", "''"),
                automezzo.modello.Replace("'", "''"),
                automezzo.kmAttuali,
                automezzo.mezzoDisponibile,
                automezzo.id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);

            return result;
        }

        public bool UpdateAutomezzo(Automezzo automezzo)
        {
            bool updated = false;
            if (ExistsAutomezzoById(automezzo.id))
                updated = EditAutomezzo(automezzo);
            else
                updated = InsertAutomezzo(automezzo);

            return updated;
        }
        public bool DeleteAutomezzo(int id)
        {
            bool result = false;
            if (CanDeleteAutomezzo(id))
            {
                string queryString = string.Format("DELETE FROM Automezzi WHERE Id = {0};", id);

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
        private bool validateAutomezzo(Automezzo automezzo, bool insertMode)
        {
            bool ok = true;
            if (automezzo.targa == null || automezzo.targa == "")
                ok = false;
            else
                if (automezzo.marca == null || automezzo.marca == "")
                    ok = false;
                else
                    if (automezzo.modello == null || automezzo.modello == "")
                        ok = false;
                    else
                        if (automezzo.kmAttuali < 0)
                            ok = false;

                            
                                
            if (insertMode)
                if(ExistsAutomezzoByTarga(automezzo.targa))
                   ok = false;
            if (!insertMode)
                if (!ExistsAutomezzoById(automezzo.id))
                    ok = false;
            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        private bool ExistsAutomezzoByTarga(string targa)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Automezzi WHERE Targa = '{0}';", targa);

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
        public bool ExistsAutomezzoById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Automezzi WHERE Id = {0};", id);

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

        private Automezzo fillAutomezzo(SqliteDataReader reader)
        {
            Automezzo automezzo = new Automezzo();
            automezzo.id = int.Parse(reader["Id"].ToString());
            automezzo.targa = reader["Targa"].ToString();
            automezzo.marca = reader["Marca"].ToString();
            automezzo.modello = reader["Modello"].ToString();
            automezzo.kmAttuali = int.Parse(reader["KmAttuali"].ToString());
            automezzo.mezzoDisponibile = reader["MezzoDisponibile"].ToString() == "1" ? true : false;

            return automezzo;
        }
        private bool CanDeleteAutomezzo(int id)
        {
            bool result = false;

            string queryString = string.Format("SELECT Prenotazioni.*, Interventi.* " +
                                               "FROM Prenotazioni " +
                                               "LEFT JOIN Interventi " +
                                               "ON Prenotazioni.IdAutomezzo = Interventi.IdAutomezzo " +
                                               "WHERE Prenotazioni.IdAutomezzo = {0} " +
                                               "UNION ALL " +
                                               "SELECT Interventi.*, Prenotazioni.* " +
                                               "FROM Interventi " +
                                               "LEFT JOIN Prenotazioni " +
                                               "ON Interventi.IdAutomezzo = Prenotazioni.IdAutomezzo " +
                                               "WHERE Interventi.IdAutomezzo = {0}", id);

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
