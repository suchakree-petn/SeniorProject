using DataPersistence;

public partial class SettingManager : IDataPersistence<InGameSettingData>
{
    public void LoadData(InGameSettingData data)
    {
        inGameSettingData.MouseSensitive = data.MouseSensitive;
    }

    public void SaveData(ref InGameSettingData data)
    {
        data.MouseSensitive = inGameSettingData.MouseSensitive;
    }
}
