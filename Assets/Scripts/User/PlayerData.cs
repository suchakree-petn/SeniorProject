using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "new Player Data", menuName = "Player Data")]
public class PlayerData : ScriptableObject, INetworkSerializable
{
    public string PlayerName;
    public ulong PlayerId;

    private static Dictionary<ulong, PlayerData> _cache;
    public static Dictionary<ulong, PlayerData> Cache
    {
        get
        {
            if (_cache == null)
            {
                _cache = new();
                PlayerData[] playerDatas = Resources.LoadAll<PlayerData>("PlayerDatas");
                _cache = playerDatas.ToDictionary(x => x.PlayerId, x => x);
            }
            return _cache;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref PlayerId);
    }
}
