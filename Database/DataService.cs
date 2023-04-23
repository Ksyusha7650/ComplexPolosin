using Database.Models;
using MySql.Data.MySqlClient;

namespace Database;

public class DataService : BaseRepository
{
    public readonly DataChannel ChannelDataBase;
    public DataEmpiricCoefficients EmpiricCoefficientsDataBase;
    public DataService()
    {
        ChannelDataBase = new DataChannel(this);
        EmpiricCoefficientsDataBase = new DataEmpiricCoefficients(this);
    }
}