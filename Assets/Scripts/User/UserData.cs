using System;
using Mono.CSharp;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct UserData : ISaveData, INetworkSerializable, IEquatable<UserData>
{
    const ulong DEFAULT_PLAYER_CHARACTER_ID = 1002;
    const ulong DEFAULT_WEAPON_ID = 200;
    public FixedString32Bytes UserName;
    public ulong UserId;
    public ulong PlayerCharacterId;

    [Header("In-Game Equipment Data")]
    public ulong WeaponId;

    public UserData(string userName, ulong userId, ulong playerCharacterId, ulong weaponId)
    {

        UserId = userId;
        UserName = (userName + "_" + userId).ToString();
        PlayerCharacterId = playerCharacterId;
        WeaponId = weaponId;
    }
    public static UserData NewData()
    {
        ulong userId = (ulong)Random.Range(ulong.MinValue, ulong.MaxValue);
        string userName = "User";
        ulong playerCharacterDataId = DEFAULT_PLAYER_CHARACTER_ID;
        ulong weaponId = DEFAULT_WEAPON_ID;
        return new(userName, userId, playerCharacterDataId, weaponId);
    }

    public bool Equals(UserData other)
    {
        return UserId == other.UserId &&
        UserName == other.UserName &&
        PlayerCharacterId == other.PlayerCharacterId &&
        WeaponId == other.WeaponId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref UserName);
        serializer.SerializeValue(ref UserId);
        serializer.SerializeValue(ref PlayerCharacterId);
        serializer.SerializeValue(ref WeaponId);
    }

}
