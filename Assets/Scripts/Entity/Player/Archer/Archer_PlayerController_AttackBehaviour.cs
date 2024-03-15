using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Archer_PlayerController
{
    [ServerRpc(RequireOwnership = false)]
    public void FireArrow_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Transform arrow = archer_playerWeapon.BowWeaponData.GetArrow(position: firePointTransform.position);
        NetworkObject arrowNetworkObject = arrow.GetComponent<NetworkObject>();
        arrowNetworkObject.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId, true);
        archer_playerWeapon.FireArrow(arrowNetworkObject);

    }
}
