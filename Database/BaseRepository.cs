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

    public async Task<int[]> GetDataByProperty(int idProperty)
    {
        for (var i = 0.0; i <= 1; i+=0.1)
        {
            Console.WriteLine(i);
        }
        
        
        const string sqlQuery = @"
select Value
from property_in_set
where ID_Property_set = @IdPropertySet
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdPropertySet", idProperty);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<int> result = new();
        while (reader.Read())
        {
            var value = reader.GetInt32(0);
            result.Add(value);
        }

        return result.ToArray();
    }
}