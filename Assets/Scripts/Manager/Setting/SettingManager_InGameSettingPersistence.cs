using DataPersistence;

public partial class SettingManager : IDataPersistence<InGameSettingData>
{
    public void LoadData(InGameSettingData data)
    {
        _inGameSettingData.MouseSensitive = data.MouseSensitive;
    }

    public void SaveData(ref InGameSettingData data)
    {
        data.MouseSensitive = _inGameSettingData.MouseSensitive;
    }
}
