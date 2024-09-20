using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Map5_ProgressInventoryManager : Singleton<Map5_ProgressInventoryManager>
{
    public Action OnAddProgress;

    [FoldoutGroup("Inventory")][SerializeField] bool isEnable;
    [FoldoutGroup("Inventory")][SerializeField] int progress;
    int prevProgress;
    [FoldoutGroup("Referrence")][SerializeField] TextMeshProUGUI progressText;
    Transform progressTextParent;
    Canvas overlayCanvas;
    float originalY;




    protected override void InitAfterAwake()
    {
        overlayCanvas = progressText.transform.parent.parent.GetComponent<Canvas>();
        progressTextParent = progressText.transform.parent;
        originalY = progressTextParent.localPosition.y;
        DisableInventory();
    }

    private void Start()
    {
        OnAddProgress += UpdateProgressUI;
        OnAddProgress += AddAnimation;
        Map5_PuzzleManager.Instance.OnFinishPanBossCamera_Local += EnableInventory;
    }

    [Button]
    private void AddAnimation()
    {
        DOTween.To(() => prevProgress, x => progressText.text = $"รวบรวม <sprite=\"wood_logs_three\" name=\"wood_logs_three\"> แล้ว <color=\"orange\">{x}</color> ชิ้น", progress, 0.7f);
        progressText.transform.DOShakePosition(0.3f, 20, 15);
        progressText.transform.DOScale(0.8f, 0.3f).SetEase(Ease.OutSine)
            .OnComplete(() => progressText.transform.DOScale(1, 0.3f).SetEase(Ease.OutBounce));
    }

    private void UpdateProgressUI()
    {
        if (!isEnable) return;

        progressText.SetText(progress.ToString());
    }

    public void GetProgess(int amount)
    {
        prevProgress = progress;
        progress += amount;

        OnAddProgress?.Invoke();
    }

    [Button]
    public void EnableInventory()
    {
        isEnable = true;

        overlayCanvas.gameObject.SetActive(true);

        float offset = -83;
        progressTextParent.DOLocalMoveY(originalY + offset, 1);
    }

    [Button]
    public void DisableInventory()
    {
        isEnable = false;

        progressTextParent.DOLocalMoveY(originalY, 1)
            .OnComplete(() =>
            {
                overlayCanvas.gameObject.SetActive(false);
            });
    }
}

