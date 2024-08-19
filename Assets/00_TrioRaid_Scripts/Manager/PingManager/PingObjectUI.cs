using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingObjectUI : MonoBehaviour
{
    [SerializeField] SpriteRenderer pingImage;
    [SerializeField] SpriteRenderer bgImage;
    [SerializeField] ulong id;
    [SerializeField] int _classId;
    [SerializeField] List<Color> classColor;
    
    public void SetPingUI(Sprite sprite,ulong ownPing, int classId){
        pingImage.sprite = sprite;
        id = ownPing;
        this._classId = SetBgColorByClass(classId);
    }
    int SetBgColorByClass(int classId){
        Color color = Color.white;
        for (int i = 0;i<3;i++){
            if(classId == i){
                color = classColor[i];
            }
        }
        bgImage.color = color;
        return classId;
    }
    public ulong GetIDPing(){
        return this.id;
    }
    public int GetClassID(){
        return this._classId;
    }
}
