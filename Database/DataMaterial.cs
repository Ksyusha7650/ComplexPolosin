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

    public int GetIdParameterSet(string name)
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

    public async void AddMaterial(PropertiesOfMaterialModel materialModel)
    {
        var idParameterSet = await _baseRepository.AddNewParameterSet();
        _baseRepository.AddParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Density"),
            await _baseRepository.GetIdUnit("Pa*s^n"),
            materialModel.Density);
        _baseRepository.AddParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Specific heat capacity"),
            await _baseRepository.GetIdUnit("J/mol"),
            materialModel.SpecificHeat);
        _baseRepository.AddParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Melting point"),
            await _baseRepository.GetIdUnit("℃"),
            materialModel.MeltingPoint);

        const string sqlQuery = @"
insert into material (ID_ParameterSet, Type)
VALUES (@IdParameterSet, @Type);
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@idParameterSet", idParameterSet);
        cmd.Parameters.AddWithValue("@Type", materialModel.Type);
        cmd.CommandText = sqlQuery;
        cmd.ExecuteNonQuery();
    }

    public async void EditMaterial(PropertiesOfMaterialModel materialModel)
    {
        var idParameterSet = GetIdParameterSet(materialModel.Type);

        _baseRepository.UpdateParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Density"),
            materialModel.Density);
        _baseRepository.UpdateParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Specific heat capacity"),
            materialModel.SpecificHeat);
        _baseRepository.UpdateParameterInParameterSet(
            idParameterSet,
            await _baseRepository.GetIdParameter("Melting point"),
            materialModel.MeltingPoint);
    }
    
    public async void DeleteMaterial(string type)
    {
        var idParameterSet = GetIdParameterSet(type);
        _baseRepository.DeleteParameterSet(idParameterSet);
        const string sqlQuery = @"
delete from material WHERE (`ID_ParameterSet` = @IdParameterSet);
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdParameterSet", idParameterSet);
        cmd.CommandText = sqlQuery;
        cmd.ExecuteNonQuery();
    }

    public async Task<PropertiesOfMaterialModel> GetMaterialProperties(int idMaterial)
    {
        const string sqlQuery = @"
select Value
from parameter_in_set ps
join parameter p on p.ID_Parameter = ps.ID_Parameter
where ID_ParameterSet = @IdMaterial and p.ID_Type = 3
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdMaterial", idMaterial);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<double> values = new();
        while (reader.Read())
        {
            var value = reader.GetDouble(0);
            values.Add(value);
        }

        return new PropertiesOfMaterialModel(
            null,
            values[0],
            MeltingPoint: values[1],
            SpecificHeat: values[2]);
    }
}