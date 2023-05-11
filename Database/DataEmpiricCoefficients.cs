using Database.Models;
using MySql.Data.MySqlClient;

namespace Database;

public class DataEmpiricCoefficients
{
    private readonly BaseRepository _baseRepository;

    public DataEmpiricCoefficients(BaseRepository baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public async Task<EmpiricCoefficientsModel[]> GetEmpiricCoefficients(int idMaterial)
    {
        const string sqlQuery = @"
select *
from parameter_in_set ps
join parameter p on p.ID_Parameter = ps.ID_Parameter
where ID_ParameterSet = @IdMaterial and p.ID_Type = 2
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdMaterial", idMaterial);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<EmpiricCoefficientsModel> empiricCoefficients = new();
        var numberEc = 1;
        while (reader.Read())
        {
            var idUnit = -1;
            if (!reader.IsDBNull(2))
                idUnit = reader.GetInt32(2);
            var unit = await _baseRepository.GetNameUnit(idUnit);
            var name = reader.GetString(5);
            var symbol = reader.GetString(6);
            var value = reader.GetDouble(3);
            empiricCoefficients.Add(new EmpiricCoefficientsModel(
                idMaterial,
                numberEc,
                name,
                unit,
                value,
                symbol));
            numberEc++;
        }

        return empiricCoefficients.ToArray();
    }

    public async Task<EmpiricCoefficientsModel> GetEmpiricCoefficient(
        int idMaterial,
        string nameEmpiricCoefficient)
    {
        const string sqlQuery = @"
select *
from parameter_in_set ps
join parameter p on p.ID_Parameter = ps.ID_Parameter
where Name = @Name and p.ID_Type = 2
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Name", nameEmpiricCoefficient);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<EmpiricCoefficientsModel> empiricCoefficients = new();
        var numberEc = 1;
        while (reader.Read())
        {
            var idUnit = -1;
            if (!reader.IsDBNull(2))
                idUnit = reader.GetInt32(2);
            var unit = await _baseRepository.GetNameUnit(idUnit);
            var name = reader.GetString(5);
            var symbol = reader.GetString(6);
            var value = reader.GetDouble(3);
            empiricCoefficients.Add(new EmpiricCoefficientsModel(
                idMaterial,
                numberEc,
                name,
                unit,
                value,
                symbol));
            numberEc++;
        }

        return empiricCoefficients[0];
    }

    public async void AddEmpiricCoefficients(int parameterSet, EmpiricCoefficientsModel empiricCoefficient)
    {
        var idParameter = await _baseRepository.AddNewParameter(
            empiricCoefficient.Name,
            empiricCoefficient.Symbol,
            2);
        var idUnit = empiricCoefficient.Unit is "" ? 8 : await _baseRepository.GetIdUnit(empiricCoefficient.Unit);
        _baseRepository.AddParameterInParameterSet(
            parameterSet,
            idParameter,
            idUnit,
            empiricCoefficient.Value);
    }

    public string[] GetNamesOfEmpiricCoefficients()
    {
        const string sqlQuery = @"
select Name
from parameter
where ID_Type = 2
";
        using var connection = _baseRepository.GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<string> empiricCoefficients = new();
        while (reader.Read()) empiricCoefficients.Add(reader.GetString(0));
        return empiricCoefficients.ToArray();
    }

    public string[] GetUnits()
    {
        const string sqlQuery = @"
select Name
from unit
";
        using var connection = _baseRepository.GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<string> units = new();
        while (reader.Read()) units.Add(reader.GetString(0));
        return units.ToArray();
    }
}