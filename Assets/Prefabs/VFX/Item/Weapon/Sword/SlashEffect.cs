using Unity.Netcode;
using UnityEngine;

public class SlashEffect : NetworkBehaviour 
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke(nameof(SelfDestroy), 1);
    }
    private void SelfDestroy()
    {
        if (!IsSpawned) return;
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
