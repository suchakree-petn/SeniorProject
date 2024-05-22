using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAction : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Ray toMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rhInfo;
            bool didHit = Physics.Raycast(toMouse,out rhInfo,500.0f);
            if(didHit){
                if(rhInfo.transform.gameObject.TryGetComponent<LobbyListSingleUI>(out LobbyListSingleUI lobbyGameObject)){
                    lobbyGameObject.JoinLobby();
                }
            }else{
                Debug.Log("click empty space");
            }
        }
    }
}
