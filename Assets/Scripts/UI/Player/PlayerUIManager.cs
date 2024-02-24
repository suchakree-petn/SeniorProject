using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : Singleton<PlayerUIManager>
{
    [Header("Reference")]
    public GameObject PlayerCanvas;
    [SerializeField] private GameObject CrossHair;

    protected override void InitAfterAwake()
    {
    }


    public void SetPlayerCrossHairState(bool active)
    {
        CrossHair.SetActive(active);
    }

}
