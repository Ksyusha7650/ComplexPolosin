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

    public async Task<int> AddNewParameter(string name, string symbol, int type)
    {
        const string sqlQuery = @"
insert into parameter (Name, Symbol, ID_Type)
values (@Name, @Symbol, @Type);
select last_insert_id();
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Symbol", symbol);
        cmd.Parameters.AddWithValue("@Type", type);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var lastId = 0;
        while (reader.Read()) lastId = reader.GetInt32(0);
        return lastId;
    }

    public async Task<int> AddNewParameterSet()
    {
        const string sqlQuery = @"
insert into parameterset
        values ();
select last_insert_id();
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var lastId = 0;
        while (reader.Read()) lastId = reader.GetInt32(0);
        return lastId;
    }

    public async void AddParameterInParameterSet(int idParameterSet, int idParameter, int? idUnit, double value)
    {
        const string sqlQuery = @"
insert into parameter_in_set (ID_ParameterSet, ID_Parameter, ID_Unit, Value)
values (@IdParameterSet, @IdParameter, @IdUnit, @Value);
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdParameterSet", idParameterSet);
        cmd.Parameters.AddWithValue("@IdParameter", idParameter);
        cmd.Parameters.AddWithValue("@IdUnit", idUnit);
        cmd.Parameters.AddWithValue("@Value", value);
        cmd.CommandText = sqlQuery;
        cmd.ExecuteNonQuery();
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
        while (reader.Read())
            result = reader.GetString(0);
        return result;
    }

    public async Task<int> GetIdUnit(string name)
    {
        const string sqlQuery = @"
select ID_Unit
from unit
where Name = @Name
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var result = 0;
        while (reader.Read())
            result = reader.GetInt32(0);
        return result;
    }

    public async Task<string> GetNameParameter(int idProperty)
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

    public async Task<int> GetIdParameter(string name)
    {
        const string sqlQuery = @"
select ID_Parameter
from parameter
where Name = @Name
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var result = 0;
        while (reader.Read())
            result = reader.GetInt32(0);
        return result;
    }
    public async void AddUnit(string name)
    {
        const string sqlQuery = @"
insert into unit (Name) values (@Name)";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.CommandText = sqlQuery;
        cmd.ExecuteNonQuery();
    }
}