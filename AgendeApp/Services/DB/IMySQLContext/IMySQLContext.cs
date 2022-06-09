using MySql.Data.MySqlClient;

namespace AgendeApp.Services.DB.IMySQLContext
{
    public interface IMySQLContext
    {
        public MySqlConnection GetConnection();
    }
}
