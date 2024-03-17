using System;
using System.Collections.Generic;
using DataPersistence;
using Unity.Netcode;
using UnityEngine;

public partial class UserManager : NetworkSingleton<UserManager>
{
    [SerializeField, ReadOnlyGUI] private UserData _userData;
    public UserData UserData => _userData;
    public DataPersistence<UserData> UserDataPersistence { get; internal set; }

    // [SerializeField] private NetworkList<UserData> AllUserData;
    protected override void InitAfterAwake()
    {
        UserDataPersistence = new("UserData", UserData.NewData());
        UserDataPersistence.LoadData();

        // AllUserData = new NetworkList<UserData>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    public override void OnNetworkSpawn()
    {

    }
    public override void OnNetworkDespawn()
    {

    }

}
