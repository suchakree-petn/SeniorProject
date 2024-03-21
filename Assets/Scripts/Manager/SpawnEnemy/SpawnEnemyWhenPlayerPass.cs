using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnEnemyWhenPlayerPass : NetworkBehaviour
{
    [SerializeField] GameObject[] enemys;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !IsOwner) return;
        if (other.transform.root.TryGetComponent<PlayerController>(out _)){
            gameObject.GetComponent<Collider>().enabled = false;
            Spawn();
        }
        
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            foreach(GameObject enemy in enemys){
                ulong id = ulong.Parse(enemy.name.Split("_")[0]);
                EnemyManager.Instance.Spawn(id,enemy.transform.position);
            }
        }
    }
    // [ServerRpc(RequireOwnership = false)]
    public void Spawn(){
        
        if(enemys != null)
            foreach(GameObject enemy in enemys){
                ulong id = ulong.Parse(enemy.name.Split("_")[0]);
                EnemyManager.Instance.Spawn(id,enemy.transform.position);
            }
    }
    
}
