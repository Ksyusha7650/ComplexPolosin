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
    
    public async void AddEmpiricCoefficients(int parameterSet, EmpiricCoefficientsModel empiricCoefficient)
    {
        var idParameter = await _baseRepository.AddNewParameter(
                empiricCoefficient.Name,
                empiricCoefficient.Symbol,
                2);
        if (empiricCoefficient.Unit != null)
                _baseRepository.AddParameterInParameterSet(
                    parameterSet,
                    idParameter,
                    await _baseRepository.GetIdUnit(empiricCoefficient.Unit),
                    empiricCoefficient.Value
                );
    }
}