using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Map2_JigsawScannerManager : NetworkSingleton<Map2_JigsawScannerManager>
{
    [SerializeField] private float scanRadius = 1;
    [SerializeField] private float scanCooldown = 4;
    private float _scanCooldown;
    [ShowInInspector] private bool isOnCooldown => _scanCooldown > 0;

    private Transform localPlayerTransform;

    [SerializeField] private string foundJigsawTextNoti;
    [SerializeField] private string notFoundJigsawTextNoti;


    [FoldoutGroup("Reference")]
    [SerializeField] private Canvas overlayCanvas;
    [FoldoutGroup("Reference")]
    [SerializeField] private TextMeshProUGUI scanNotificationText;

    [FoldoutGroup("Reference")]
    [InlineEditor]
    [SerializeField] private Transform scanDummy;

    [FoldoutGroup("Reference")]
    [SerializeField] private Transform scanAuraVFX_prf;
    [FoldoutGroup("Reference")]
    [SerializeField] private Image scanIcon;


    protected override void InitAfterAwake()
    {
        _scanCooldown = scanCooldown;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        localPlayerTransform = PlayerManager.Instance.LocalPlayerController.transform;

    }


    private void Update()
    {
        _scanCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && !isOnCooldown)
        {
            _scanCooldown = scanCooldown;
            ScanArea(localPlayerTransform.position);
        }

        scanIcon.fillAmount = 1 - _scanCooldown / scanCooldown;
    }

    public void ScanArea(Vector3 position)
    {
        scanNotificationText.DOFade(0, 0f);
        scanNotificationText.DOKill();

        Sequence sequence = DOTween.Sequence();
        Instantiate(scanAuraVFX_prf, position, Quaternion.identity);
        ScanArea_ServerRpc(position, NetworkManager.LocalClientId);

        bool isFound = false;

        RaycastHit[] raycastHit = Physics.SphereCastAll(position, scanRadius, -localPlayerTransform.up, 0.1f);
        foreach (RaycastHit hit in raycastHit)
        {
            Debug.Log("hit: "+hit.collider.name);
            if (hit.collider.TryGetComponent(out ICollectable collectable))
            {
                isFound = true;
                collectable.Collect();
                scanNotificationText.text = foundJigsawTextNoti;
                break;
            }
        }
        if (!isFound)
            scanNotificationText.text = notFoundJigsawTextNoti;

        sequence.Append(scanNotificationText.DOFade(1, 0.3f));
        sequence.AppendInterval(3);
        sequence.Append(scanNotificationText.DOFade(0, 1f));
        sequence.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ScanArea_ServerRpc(Vector3 position, ulong clientId)
    {
        ScanArea_ClientRpc(position, clientId);
    }

    [ClientRpc]
    public void ScanArea_ClientRpc(Vector3 position, ulong clientId)
    {
        if (NetworkManager.LocalClientId == clientId) return;

        Instantiate(scanAuraVFX_prf, position, Quaternion.identity);

    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Vector3 origin;

        if (localPlayerTransform)
        {
            origin = PlayerManager.Instance.LocalPlayerController.transform.position;
        }
        else
        {
            if (!scanDummy) return;
            origin = scanDummy.transform.position;
        }

        Gizmos.DrawWireSphere(origin, scanRadius);

    }
}

