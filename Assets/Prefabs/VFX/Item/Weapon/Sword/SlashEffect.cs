using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SlashEffect : NetworkBehaviour 
{
    public Transform _position;
    private void Update() {
        transform.position = _position.position;
    }
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
