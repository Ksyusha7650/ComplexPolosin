using Database.Interfaces;
using MySql.Data.MySqlClient;

namespace Database;

public abstract class BaseRepository : IDbRepository
{
    private readonly string _connectionString =
        "host=localhost;port=3306;database=models_for_pc;username=root;password=04042002Mm!";

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
        const string sqlQuery = @"
select Value
from property_in_set
where ID_PropertySet = @IdPropertySet
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