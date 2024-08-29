using System;
using Cinemachine;
using DG.Tweening;
using Gamekit3D;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Broken_BalistaController : NetworkBehaviour
{
    [Header("Broken")]
    public Action OnRepairSuccess;

    [SerializeField] private NetworkVariable<float> repairProgress = new(0);
    public float RepairProgress => repairProgress.Value;
    public float RepairMaxProgress = 100;
    public bool IsRepaired => RepairProgress >= RepairMaxProgress;


    [Header("Refererence")]
    [SerializeField] private TextMeshPro text_interactButton;
    [SerializeField] private InteractOnButton use_interactButton;
    [SerializeField] private InteractOnButton exit_interactButton;
    [SerializeField] private Animator animator;

    private void Start()
    {
        text_interactButton.SetText("<color=#ffa500ff> F </color> เพื่อซ่อม");

    }

    private void Update()
    {
        animator.SetFloat("RepairProgress", RepairProgress / RepairMaxProgress);
    }

   

    private void OnEnable()
    {
        use_interactButton.OnEnter.AddListener(ShowInteractText);
        use_interactButton.OnExit.AddListener(HideInteractText);

    }

    private void OnDisable()
    {
        use_interactButton.OnEnter.RemoveListener(ShowInteractText);
        use_interactButton.OnExit.RemoveListener(HideInteractText);

    }



    private void HideInteractText()
    {
        text_interactButton.gameObject.SetActive(false);
    }

    private void ShowInteractText()
    {
        text_interactButton.gameObject.SetActive(true);
    }
}
