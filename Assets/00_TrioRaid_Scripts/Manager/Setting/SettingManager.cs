using System;
using DataPersistence;
using UnityEngine;

public partial class SettingManager : Singleton<SettingManager>
{
    public InGameSettingData InGameSettingData;
    public DataPersistence<InGameSettingData> dataPersistenceInGameSetting;

    public Action<InGameSettingData, InGameSettingData> OnSettingChanged;

    public bool IsDataLoaded;
    protected override void InitAfterAwake()
    {
        InGameSettingData = new();
        dataPersistenceInGameSetting = new("InGameSetting", InGameSettingData);

        dataPersistenceInGameSetting.OnLoadSuccess += () => IsDataLoaded = true;
    }

    private void Start()
    {
        // dataPersistenceInGameSetting.LoadData();
    }


}
