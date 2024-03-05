using DataPersistence;
using UnityEngine;

public partial class UserManager : Singleton<UserManager>
{
    [SerializeField,ReadOnlyGUI] private UserData _userData;
    public UserData UserData => _userData;
    public DataPersistence<UserData> UserDataPersistence { get; internal set; }

    protected override void InitAfterAwake()
    {
        _userData = UserData.NewData();
        UserDataPersistence = new("UserData",_userData);
    }

    private void Start()
    {
        // UserDataPersistence.SaveData();
        UserDataPersistence.LoadData();
    }

}
