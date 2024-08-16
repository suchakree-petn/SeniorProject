using DataPersistence;

public partial class SettingManager : IDataPersistence<InGameSettingData>
{
    public void LoadData(InGameSettingData data)
    {
        InGameSettingData.MouseSensitive_ThirdPerson = data.MouseSensitive_ThirdPerson;
        InGameSettingData.MouseSensitive_Focus = data.MouseSensitive_Focus;
    }

    public void SaveData(ref InGameSettingData data)
    {
        data.MouseSensitive_ThirdPerson = InGameSettingData.MouseSensitive_ThirdPerson;
        data.MouseSensitive_Focus = InGameSettingData.MouseSensitive_Focus;
    }
}
