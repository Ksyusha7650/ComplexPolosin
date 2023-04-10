using System.Configuration;
using Database.Interfaces;
using MySql.Data.MySqlClient;

namespace Database;

public abstract class BaseRepository : IDbRepository
{
    private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MyDatabaseCS"].ConnectionString;

    public async Task<MySqlConnection> GetAndOpenConnection()
    {
        try
        {
            var conn = new MySqlConnection();
            conn.ConnectionString = _connectionString;
            await conn.OpenAsync();
            return conn;
        }
        catch (MySqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }
}