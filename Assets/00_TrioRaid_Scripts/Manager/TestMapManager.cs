using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestMapManager : NetworkSingleton<TestMapManager>
{
    public PlayerController playerController;

    protected override void InitAfterAwake()
    {

    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        NetworkManager.StartHost();
        if (playerController)
        {
            if (playerController.IsSpawned)
            {
                GameObject player = Instantiate(playerController.gameObject, playerController.transform.position, Quaternion.identity);
                PlayerController newPlayerController = player.GetComponent<PlayerController>();

                playerController.NetworkObject.Despawn();
                newPlayerController.NetworkObject.SpawnAsPlayerObject(NetworkManager.LocalClientId);
                playerController = newPlayerController;
            }
            else
            {
                playerController.NetworkObject.SpawnAsPlayerObject(playerController.OwnerClientId);

            }
        }
    }

    public override void OnNetworkSpawn()
    {

    }
}
