﻿using Database.Models;
using MySql.Data.MySqlClient;

namespace Database;

public class DataService : BaseRepository
{
    public string[] GetMarks()
    {
        const string sqlQuery = @"
select Mark
  from channel
";
        using var connection = GetAndOpenConnection().Result;
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        List<string> marks = new();
        while (reader.Read()) marks.Add(reader.GetString(0));
        return marks.ToArray();
    }

    public async Task<GeometricParametersModel> GetGeometricParameters(string mark)
    {
        const string sqlQuery = @"
select ID_PropertySet
  from channel
 where Mark = @Mark
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Mark", mark);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var idProSet = 0;
        while (reader.Read())
            idProSet = reader.GetInt32(0);
        var values = await GetDataByProperty(idProSet);
        return new GeometricParametersModel(values[0], values[1], values[2]);
    }

    public async void SetGeometricParameters(
        string mark,
        GeometricParametersModel geometricParametersModel)
    {
        const string sqlQuery = @"
insert into table channel
set Mark = @Mark, ID_PropertySet = @IdProSet
";
        await using var connection = await GetAndOpenConnection();
        var cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.Parameters.AddWithValue("@Mark", mark);
        cmd.CommandText = sqlQuery;
        var reader = cmd.ExecuteReader();
        var idProSet = 0;
        while (reader.Read()) idProSet = reader.GetInt32(0);

        var values = await GetDataByProperty(idProSet);
    }
}