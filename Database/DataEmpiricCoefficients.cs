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
from empiriccoefficients
where ID_material = @IdMaterial
";
        await using var connection = await _baseRepository.GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@IdMaterial", idMaterial);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<EmpiricCoefficientsModel> empiricCoefficients = new();
        while (reader.Read())
        {
            var idEc = reader.GetInt32(1);
            var idUnit = -1;
            if (!reader.IsDBNull(2))
                idUnit = reader.GetInt32(2);
            var unit = await _baseRepository.GetNameUnit(idUnit);
            var idProperty = reader.GetInt32(3);
            var property = await _baseRepository.GetNameProperty(idProperty);
            var value = reader.GetDouble(4);
            empiricCoefficients.Add(new EmpiricCoefficientsModel(
                idMaterial,
                idEc,
                property,
                unit,
                value));
        }

        return empiricCoefficients.ToArray();
    }
}