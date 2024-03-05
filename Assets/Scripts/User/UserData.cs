using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class UserData : ISaveData
{
    public string UserName;
    public ulong UserId;

    public UserData(string userName, ulong userId)
    {

        UserId = userId;
        UserName = userName + " " + userId;
    }
    public static UserData NewData()
    {
        ulong userId = (ulong)Random.Range(ulong.MinValue, ulong.MaxValue);
        string userName = "User_";
        return new(userName, userId);
    }


    // private static Dictionary<ulong, UserData> _cache;
    // public static Dictionary<ulong, UserData> Cache
    // {
    //     get
    //     {
    //         if (_cache == null)
    //         {
    //             _cache = new();
    //             UserData[] UserDatas = Resources.LoadAll<UserData>("UserDatas");
    //             _cache = UserDatas.ToDictionary(x => x.UserId, x => x);
    //         }
    //         return _cache;
    //     }
    // }

    // public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    // {
    //     serializer.SerializeValue(ref UserName);
    //     serializer.SerializeValue(ref UserId);
    // }

}
