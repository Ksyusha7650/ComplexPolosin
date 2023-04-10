using Database.Models;

namespace Database.Interfaces;

public interface IDbQueries
{
    Task<GeometricParameters> GetGeometricParameters(string mark);
}