using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinValidator.Editor;
using Unity.Netcode;
using UnityEngine;

public class Map5_HornManager : NetworkSingleton<Map5_HornManager>
{
    public GameObject CurrentActiveHorn;

    [SerializeField] private List<GameObject> horn = new();

    [FoldoutGroup("Reference")]
    [SerializeField] private CinemachineVirtualCamera bossCam;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform bossSpawn;

    protected override void InitAfterAwake()
    {
    }

}

