using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PingMenuManager : MonoBehaviour
{
    public GameObject pingMenuRootObject;
    public Transform CursorRootObject;
    public Transform HighlightRootObject;
    void Start()
    {
        pingMenuRootObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2)){
            pingMenuRootObject.SetActive(true);
        }
        if(Input.GetMouseButtonUp(2)){
            pingMenuRootObject.SetActive(false);
        }
        if(Input.GetMouseButton(2)){
            // Vector2 MouseDelta;
            // MouseDelta = new Vector2(Input.mousePosition.x, Input.mousePosition.x);
            
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 mouseDelta = mousePosition - screenCenter;

            float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;

            float SmoothAngle = Mathf.MoveTowardsAngle(CursorRootObject.eulerAngles.z, angle, mouseDelta.magnitude * Time.deltaTime*250);
            CursorRootObject.transform.eulerAngles = new Vector3(0,0,SmoothAngle);

            float highlightAngle = Mathf.Round(SmoothAngle /60);
            HighlightRootObject.transform.eulerAngles = new Vector3(0,0,highlightAngle);
        }
    }
}
