using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Map4Manager : NetworkSingleton<Map4Manager>
{
[SerializeField] GameObject bossIdle;
    protected override void InitAfterAwake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // [ServerRpc]
    [ClientRpc]
    public void DestroyBossIdleClientRpc(){
        Destroy(bossIdle);
    }
}
