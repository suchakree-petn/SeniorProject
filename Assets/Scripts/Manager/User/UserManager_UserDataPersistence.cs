using DataPersistence;

public partial class UserManager : IDataPersistence<UserData>
{
    public void LoadData(UserData data)
    {
        _userData.UserName = data.UserName;
        _userData.UserId = data.UserId;
    }

    public void SaveData(ref UserData data)
    {
        data.UserName =  _userData.UserName;
        data.UserId =  _userData.UserId;
    }

}
