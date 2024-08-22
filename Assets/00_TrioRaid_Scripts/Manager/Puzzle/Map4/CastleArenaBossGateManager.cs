using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleArenaBossGateManager : MonoBehaviour
{
    [SerializeField]GateController gateControllerBoss;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private List<GameObject> boss;
    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        if (boss.Count == 0 && timer>3){
            gateControllerBoss.CloseGate();
            gameObject.SetActive(false);
        }else if(timer<=3){
            timer++;
        }
        Debug.Log(boss.Count);
    }
    void OnTriggerEnter(Collider other) {
        
        Debug.Log("ColliderEnter");
        
        if(0 != (enemyLayers.value & 1 << other.gameObject.transform.root.gameObject.layer)){
            Debug.Log("Enter "+other.gameObject.transform.root.gameObject.name);
            boss.Add(other.gameObject.transform.root.gameObject);
        }
        
    }
        
    void OnTriggerExit(Collider other) {
        
        Debug.Log("ColliderExit");
        
        if(0 != (enemyLayers.value & 1 << other.gameObject.transform.root.gameObject.layer)){
            Debug.Log("Exit "+other.gameObject.transform.root.gameObject.name);
            boss.Remove(other.gameObject.transform.root.gameObject);
        }
        
    }
}
