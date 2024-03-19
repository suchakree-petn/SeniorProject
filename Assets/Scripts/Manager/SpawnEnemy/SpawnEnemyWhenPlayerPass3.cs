using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnEnemyWhenPlayerPass3 : NetworkBehaviour
{
    [SerializeField] GameObject[] enemys;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !IsOwner) return;
        if (other.transform.root.TryGetComponent<PlayerController>(out _)){
            gameObject.GetComponent<Collider>().enabled = false;
            Spawn_ServerRpc();
        }
        
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            foreach(GameObject enemy in enemys){
                EnemyManager.Instance.Spawn(2002,enemy.transform.position);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void Spawn_ServerRpc(){
        
        if(enemys != null)
            foreach(GameObject enemy in enemys){
                enemy.SetActive(true);
                enemy.GetComponent<NetworkObject>().Spawn(true);
                // StartCoroutine(DelayActiveObj(enemy,0.5f));
            }
    }
    // public override void OnNetworkSpawn()
    // {
    //     if (!IsServer) return;
    //     if(enemys != null)
    //     foreach(GameObject enemy in enemys){
    //         enemy.SetActive(false);
    //         enemy.GetComponent<NetworkObject>().Despawn();
    //     }
    // }
    // private void Start() {
        
    // }
    IEnumerator DelayActiveObj(GameObject enemy,float time){
        yield return new WaitForSeconds(time);
        enemy.SetActive(true);
        enemy.GetComponent<NetworkObject>().Spawn();
        
    }
    
}
