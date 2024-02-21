using System;
using DataPersistence;
using UnityEngine;

public partial class SettingManager : Singleton<SettingManager>
{
    [SerializeField] private InGameSettingData inGameSettingData;
    public DataPersistence<InGameSettingData> dataPersistenceInGameSetting;
    private bool isInitSetting;

    public Action OnSettingInit;
    public Action<InGameSettingData, InGameSettingData> OnSettingChanged;


    protected override void InitAfterAwake()
    {
        OnSettingInit += () => isInitSetting = true;
        dataPersistenceInGameSetting = new("InGameSetting", "SAVE", new());
    }

    private void Start()
    {
        // dataPersistenceInGameSetting.LoadData();
    }
}
