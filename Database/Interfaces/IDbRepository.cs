using MySql.Data.MySqlClient;

namespace Database.Interfaces;

public interface IDbRepository
{
    Task<MySqlConnection> GetAndOpenConnection();
}