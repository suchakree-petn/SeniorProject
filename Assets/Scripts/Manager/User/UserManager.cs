using DataPersistence;
using UnityEngine;

public partial class UserManager : Singleton<UserManager>
{
    [SerializeField, ReadOnlyGUI] private UserData _userData;
    public UserData UserData => _userData;
    public DataPersistence<UserData> UserDataPersistence { get; internal set; }

    protected override void InitAfterAwake()
    {
        UserDataPersistence = new("UserData", UserData.NewData());
        UserDataPersistence.LoadData();

        // if (_userData == null)
        // {
        // }

        // // if still not found local data, then create new and save
        // if (_userData == null)
        // {
        //     Debug.LogWarning($"New user data, Saving...");
        //     _userData = UserData.NewData();
        //     UserDataPersistence.SaveData();
        // }

    }

    private void Start()
    {
        // UserDataPersistence.SaveData();
    }

}
