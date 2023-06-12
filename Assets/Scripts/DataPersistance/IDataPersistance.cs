public interface IDataPersistance
{
    void LoadData(GameData gameData);
    void SaveData(ref GameData gameData);
}