using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CastleArenaPlayerGateManager : MonoBehaviour
{
    [SerializeField]GateController gateControllerPlayer;
    [SerializeField]GateController gateControllerBoss;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private List<GameObject> players;
    [SerializeField] private Transform bossSpawn;
        
    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        if (players.Count == 0 && timer>3){
            gateControllerPlayer.CloseGate();
            gateControllerBoss.OpenGate();
            if(NetworkManager.Singleton.IsServer){
            Map4Manager.Instance.DestroyBossIdleClientRpc();
            EnemyManager.Instance.Spawn(2005,bossSpawn.position);
        }
            gameObject.SetActive(false);
        }else if(timer<=3){
            timer += Time.deltaTime;
        }
        Debug.Log(players.Count);
    }
    void OnTriggerEnter(Collider other) {
        
        Debug.Log("ColliderEnter");
        
        if(0 != (playerLayers.value & 1 << other.gameObject.transform.root.gameObject.layer)){
            Debug.Log("Enter "+other.gameObject.transform.root.gameObject.name);
            players.Add(other.gameObject.transform.root.gameObject);
        }
        
    }
        
    void OnTriggerExit(Collider other) {
        
        Debug.Log("ColliderExit");
        
        if(0 != (playerLayers.value & 1 << other.gameObject.transform.root.gameObject.layer)){
            Debug.Log("Exit "+other.gameObject.transform.root.gameObject.name);
            players.Remove(other.gameObject.transform.root.gameObject);
        }
        
    }
}
