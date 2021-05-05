using Microsoft.Data.Sqlite;
using NoleggioAutomezzi.Exceptions;
using NoleggioAutomezzi.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NoleggioAutomezzi.Repository
{
    public class UtentiRepository
    {
        private DatabaseConnectionRepository _repoDatabase;
        private MailRepository _repoMail;

        public UtentiRepository()
        {
            _repoDatabase = new DatabaseConnectionRepository();
            _repoMail = new MailRepository();
        }

        public Utente GetUtenteById(int id)
        {
            Utente utente = null;
            string queryString = string.Format("SELECT * FROM Utenti WHERE Id = {0};", id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    utente = fillUtente(reader);
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            return utente;
        }
        public Utente Login(string username, string password)
        {
            Utente utente = null;
            byte[] b = System.Text.Encoding.ASCII.GetBytes(password);
            string encrypted = Convert.ToBase64String(b);
            string queryString = string.Format("SELECT * FROM Utenti WHERE Username = '{0}' AND Password = '{1}';", username, encrypted);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    utente = fillUtente(reader);
                else
                    throw new LoginFailedException();
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }
            
            return utente;
        }
        public Utente CheckLogin(int userId, string password)
        {
            Utente utente = null;

            string queryString = string.Format("SELECT * FROM Utenti WHERE Id = {0} AND Password = '{1}';", userId, password);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            SqliteDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.Read())
                    utente = fillUtente(reader);
                else
                    throw new LoginFailedException();
            }
            finally
            {
                reader.Close();
                _repoDatabase.Close(connection);
            }

            return utente;
        }
        public bool InsertUtente(Utente utente)
        {
            validateUtente(utente, true);
            byte[] b = System.Text.Encoding.ASCII.GetBytes(utente.password);
            string encrypted = Convert.ToBase64String(b);
            string queryString = string.Format("INSERT INTO Utenti(Nome,Cognome,Username,DataNascita,Password, HasPermessi, indirizzoEmail) values('{0}','{1}','{2}','{3}','{4}', 0, '{5}');", 
                utente.nome.Replace("'","''"),
                utente.cognome.Replace("'","''"),
                utente.username.Replace("'","''"),
                utente.dataNascita.ToString("yyyy-MM-dd"),
                encrypted.Replace("'","''"),
                utente.indirizzoEmail.Replace("'","''"));

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);
            if (result)
                _repoMail.SendMail(utente.indirizzoEmail, "Benvenuto in Noleggio Automezzi",
                    string.Format("Ciao {0}, ti confermo la registrazione al servizio di noleggio automezzi!\nDi seguito i dati che hai inserito:\nNome:{0};\nCognome:{1};\nusername:{2};\n\n\nDiego Smacchia\nNoleggio Automezzi", utente.nome, utente.cognome, utente.username));
            return result;
        }
        public bool EditUtente(Utente utente)
        {
            validateUtente(utente, false);
            byte[] b = System.Text.Encoding.ASCII.GetBytes(utente.password);
            string encrypted = Convert.ToBase64String(b);
            string queryString = string.Format("UPDATE Utenti SET Nome = '{0}',Cognome = '{1}',Username = '{2}', DataNascita = '{3}',Password = '{4}' WHERE Id = {5};",
                utente.nome.Replace("'", "''"),
                utente.cognome.Replace("'", "''"),
                utente.username.Replace("'", "''"),
                utente.dataNascita.ToString("yyyy-MM-dd"),
                encrypted.Replace("'", "''"),
                utente.id);

            SqliteConnection connection = _repoDatabase.Connect();
            SqliteCommand command = new SqliteCommand(queryString, connection);

            int res = command.ExecuteNonQuery();
            bool result = res == 1 ? true : false;

            _repoDatabase.Close(connection);

            return result;
        }
        public bool UpdateUtente(Utente utente)
        {
            bool updated = false;
            if (ExistsUtenteById(utente.id))
                updated = EditUtente(utente);
            else
                updated = InsertUtente(utente);

            return updated;
        }
        private bool validateUtente(Utente utente, bool insertMode)
        {
            bool ok = true;
            if (utente.nome == null || utente.nome == "")
                ok = false;
            else
                if (utente.cognome == null || utente.cognome == "")
                    ok = false;
                else
                    if (utente.username == null || utente.username == "")
                        ok = false;
                    else
                        if (utente.password == null || utente.password == "")
                            ok = false;
                        else
                            if (utente.indirizzoEmail == null || utente.indirizzoEmail == "")
                                ok = false;

            if (insertMode)
                if (ExistsUtenteByUsername(utente.username))
                    ok = false;
            if (!insertMode)
                if (!ExistsUtenteById(utente.id))
                    ok = false;
            if (!ok)
                throw new InvalidModelException();

            return ok;
        }
        public bool ExistsUtenteByUsername(string username)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Utenti WHERE username = '{0}';", username);

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
        public bool ExistsUtenteByIndirizzoEmail(string indirizzoEmail)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Utenti WHERE IndirizzoEmail = '{0}';", indirizzoEmail);

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
        public bool ExistsUtenteById(int id)
        {
            bool found = false;
            string queryString = string.Format("SELECT * FROM Utenti WHERE Id = {0};", id);

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
        private Utente fillUtente(SqliteDataReader reader)
        {
            Utente utente = new Utente();
            utente.id = int.Parse(reader["Id"].ToString());
            utente.nome = reader["Nome"].ToString();
            utente.cognome = reader["Cognome"].ToString();
            utente.username = reader["Username"].ToString();
            utente.password = reader["Password"].ToString();
            utente.dataNascita = DateTime.Parse(reader["DataNascita"].ToString());
            utente.hasPermessi = (reader["HasPermessi"].ToString() == "1");
            utente.indirizzoEmail = reader["IndirizzoEmail"].ToString();

            return utente;
        }
    }
}
