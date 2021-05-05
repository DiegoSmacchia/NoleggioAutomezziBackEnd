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
    public class DatabaseConnectionRepository
    {
        SqliteConnection connection;
        string connectionString;
        public DatabaseConnectionRepository()
        {
            connectionString = new SqliteConnectionStringBuilder("Data Source='Database/NoleggioAutomezzi.db'")
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = ""
            }.ToString();

        }

        public SqliteConnection Connect()
        {
            connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        }
        public SqliteConnection Close(SqliteConnection connection)
        {
            connection.Close();
            return connection;
        }
    }
}
