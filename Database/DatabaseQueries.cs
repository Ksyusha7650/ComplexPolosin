using Database.Interfaces;
using Database.Models;

namespace Database;

public class DatabaseQueries : BaseRepository, IDbQueries
{
    public async Task<GeometricParameters> GetGeometricParameters(string mark)
    {
        const string sqlQuery = @"
select ID_PropertySet
  from channel
 where Mark = @Mark
";

        var sqlQueryParams = new
        {
            Mark = mark
        };

        await using var connection = await GetAndOpenConnection();
        return new GeometricParameters(1, 2, 3);
    }
}