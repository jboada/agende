using MySql.Data.MySqlClient;
using System;

namespace AgendeApp.Services.DB.IMySQLContext
{
    public class MySQLContext : IMySQLContext
    {
        public string ConnectionString { get; set; }

        public MySQLContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
