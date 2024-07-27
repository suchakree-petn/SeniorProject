using DataPersistence;

public partial class UserManager : IDataPersistence<UserData>
{
    public void LoadData(UserData data)
    {
        _userData.UserName = data.UserName;
        _userData.UserId = data.UserId;
        _userData.PlayerCharacterId = data.PlayerCharacterId;
        _userData.WeaponId = data.WeaponId;
    }

    public void SaveData(ref UserData data)
    {
        data.UserName =  _userData.UserName;
        data.UserId =  _userData.UserId;
        data.PlayerCharacterId =  _userData.PlayerCharacterId;
        data.WeaponId =  _userData.WeaponId;
    }

}
