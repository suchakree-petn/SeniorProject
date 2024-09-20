using System;
using Cinemachine;
using DG.Tweening;
using Gamekit3D;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Broken_BalistaController : NetworkBehaviour
{


    [FoldoutGroup("Config")] public Action OnRepairSuccess;

    [FoldoutGroup("Config")][SerializeField] NetworkVariable<float> repairProgress = new(0);
    public float RepairProgress => repairProgress.Value;

    [FoldoutGroup("Config")][SerializeField] float RepairMaxProgress = 100;
    public bool IsRepaired => RepairProgress >= RepairMaxProgress;


    [FoldoutGroup("Reference")][SerializeField] TextMeshProUGUI interactButton;
    [FoldoutGroup("Reference")][SerializeField] Animator animator;

    Canvas canvas;
    OutlineController outlineController;

    private void Awake()
    {
        outlineController = GetComponent<OutlineController>();

    }

    private void Start()
    {
        canvas = interactButton.transform.parent.GetComponent<Canvas>();
        canvas.gameObject.SetActive(true);
        interactButton.gameObject.SetActive(false);

    }


    private void Update()
    {
        animator.SetFloat("RepairProgress", RepairProgress / RepairMaxProgress);

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartUsing();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {

            StopUsing();
        }
    }


    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController player)) return;

        if (!player.IsLocalPlayer) return;

        ShowInteractText();


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController player)) return;

        if (!player.IsLocalPlayer) return;

        interactButton.gameObject.SetActive(false);

        HideInteractText();



    }

    private void StopUsing()
    {

    }

    private void StartUsing()
    {

    }


    private void HideInteractText()
    {
        interactButton.gameObject.SetActive(false);
    }

    private void ShowInteractText()
    {
        interactButton.gameObject.SetActive(true);
    }

    private void CheckShowOutline(Map5_GameState state)
    {
        if (state == Map5_GameState.Phase2_RepairBalista)
        {
            outlineController.ShowOutline();
        }
        else
        {
            outlineController.HideOutline();
        }
    }
}
