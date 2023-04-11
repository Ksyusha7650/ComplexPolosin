using Database.Models;

namespace Database.Interfaces;

public interface IDbQueries
{
    Task<GeometricParametersModel> GetGeometricParameters(string mark);
}