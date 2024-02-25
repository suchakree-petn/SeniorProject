using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "new Player Data", menuName = "Player Data")]
public class PlayerData : ScriptableObject, INetworkSerializable
{
    public string PlayerName;
    public ulong PlayerId;

    [Header("Info")]
   public string _name;
   public int _level;
   public LayerMask targetLayer;

   [Header("Movement")]
   public float _moveSpeed;

   [Header("Health Point")]
   public float _hpBase;
   public float _hpBonus;

   [Header("Attack Point")]
   public float _attackBase;
   public float _attackBonus;

   [Header("Defense Point")]
   public float _defenseBase;
   public float _defenseBonus;

   public float GetMaxHp(){
      return _hpBase + _hpBonus;
   }
   public float GetAttack(){
      return _attackBase + _attackBonus;
   }


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
