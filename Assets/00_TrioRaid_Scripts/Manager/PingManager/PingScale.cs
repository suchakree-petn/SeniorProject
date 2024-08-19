using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PingScale : MonoBehaviour
{
    public float scaleFactor;
    public float minDist;
    public Transform playerCamera;
    // public TextMeshPro distText;
    float dist;
    void Start()
    {
        playerCamera = Camera.main.GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(playerCamera);
        dist = Vector3.Distance(transform.position, playerCamera.transform.position);
        if (dist > minDist){
            transform.localScale = Vector3.one * dist / minDist * scaleFactor;
        }else{
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
