using Database.Interfaces;
using MySql.Data.MySqlClient;

namespace Database;

public abstract class BaseRepository : IDbRepository
{
    private readonly string _connectionString =
        "host=localhost;port=3306;database=programcomplex;username=root;password=04042002Mm!";

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

    public async Task<double[]> GetDataByPropertySet(int idPropertySet)
    {
        const string sqlQuery = @"
select Value
from parameter_in_set
where ID_ParameterSet = @IdPropertySet
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdPropertySet", idPropertySet);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<double> result = new();
        while (reader.Read())
        {
            var value = reader.GetDouble(0);
            result.Add(value);
        }

        return result.ToArray();
    }

    public async Task<string> GetNameUnit(int idUnit)
    {
        const string sqlQuery = @"
select Name
from unit
where ID_Unit = @IdUnit
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdUnit", idUnit);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        string result = null;
        while (reader.Read()) result = reader.GetString(0);

        return result;
    }

    public async Task<string> GetNameProperty(int idProperty)
    {
        const string sqlQuery = @"
select Name
from parameter
where ID_Parameter = @IdProperty
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdProperty", idProperty);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        string result = null;
        while (reader.Read()) result = reader.GetString(0);
        return result;
    }
}