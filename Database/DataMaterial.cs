using Database.Models;
using MySql.Data.MySqlClient;

namespace Database;

public class DataMaterial
{
    private readonly BaseRepository _baseRepository;

    public DataMaterial(BaseRepository baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public string[] GetTypes()
    {
        const string sqlQuery = @"
select Type
from material
";
        using var connection = _baseRepository.GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<string> types = new();
        while (reader.Read()) types.Add(reader.GetString(0));
        return types.ToArray();
    }

    public int GetIdMaterial(string name)
    {
        const string sqlQuery = @"
select ID_ParameterSet
from material
where Type = @TypeName;
";
        using var connection = _baseRepository.GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@TypeName", name);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var idMaterial = 0;
        while (reader.Read())
            idMaterial = reader.GetInt32(0);
        return idMaterial;
    }
    
}