using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class JigsawMapStandController : MonoBehaviour
{
    public bool IsInUse = false;




    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject worldSpaceCanvas;
    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject overlayCanvas;
    [FoldoutGroup("Reference")]
    [SerializeField] private TextMeshProUGUI interactButtonText;

    private void Start()
    {
        worldSpaceCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        worldSpaceCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        StopUseJigsawStand();
        worldSpaceCanvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartUseJigsawStand();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            StopUseJigsawStand();
        }


    }

    [HideIf("IsInUse")]
    [Button("Use")]
    public void StartUseJigsawStand()
    {
        if (!IsInUse)
        {
            IsInUse = true;
            ShowJigsawSpotMap();
            interactButtonText.gameObject.SetActive(false);
        }
    }

    [ShowIf("IsInUse")]
    [Button("Exit")]
    public void StopUseJigsawStand()
    {
        if (IsInUse)
        {
            IsInUse = false;
            HideJigsawSpotMap();
            interactButtonText.gameObject.SetActive(true);
        }
    }

    private void ShowJigsawSpotMap()
    {
        overlayCanvas.SetActive(true);
    }

    private void HideJigsawSpotMap()
    {
        overlayCanvas.SetActive(false);
    }

}
