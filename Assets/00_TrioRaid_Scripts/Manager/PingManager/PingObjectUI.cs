using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingObjectUI : MonoBehaviour
{
    [SerializeField] SpriteRenderer pingImage;
    [SerializeField] ulong id;

    public void SetPingUI(Sprite sprite,ulong ownPing){
        pingImage.sprite = sprite;
        id = ownPing;
        Debug.Log(ownPing);
    }
    public ulong GetIDPing(){
        return this.id;
    }
}
