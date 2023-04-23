namespace Database;

public class DataService : BaseRepository
{
    public readonly DataChannel ChannelDataBase;
    public readonly DataEmpiricCoefficients EmpiricCoefficientsDataBase;
    public readonly DataMaterial MaterialDataBase;

    public DataService()
    {
        MaterialDataBase = new DataMaterial(this);
        ChannelDataBase = new DataChannel(this);
        EmpiricCoefficientsDataBase = new DataEmpiricCoefficients(this);
    }
}