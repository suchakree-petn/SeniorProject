using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : NetworkSingleton<PlayerUIManager>
{
    [SerializeField] private float FS_Hit_Intensity = 0.1f;
    [SerializeField] private float FS_Hit_Duration = 0.3f;
    [SerializeField] private float fadeDuration = 4;

    [Header("In Game Reference")]
    public GameObject PlayerCanvas;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private GameObject lockTarget;
    [SerializeField] private GameObject respawnCountdownUI;
    [SerializeField] private TextMeshProUGUI respawnCountdownUI_text;
    [SerializeField] private GameObject resultUI;
    [SerializeField] private Material fullScreen_Hit;

    public float RespawnCountdown = 10;
    private float respawnCountdown;
    private bool isShowRespawnUI;

    [FoldoutGroup("Reference select UI")]
    [SerializeField] private GameObject selectCharacterMenu;
    [FoldoutGroup("Reference select UI")]
    [SerializeField] private TMP_Dropdown selectCharacterDropdown;
    [FoldoutGroup("Reference select UI")]
    [SerializeField] private Button selectCharacterButton_Host;
    [FoldoutGroup("Reference select UI")]
    [SerializeField] private Button selectCharacterButton_Client;
    [FoldoutGroup("Reference select UI")]
    [SerializeField] private Image fadeScreen;


    private void Update()
    {
        if (isShowRespawnUI)
        {
            respawnCountdown -= Time.deltaTime;
            respawnCountdownUI_text.text = "Respawn in: " + (int)respawnCountdown;
        }
    }
    public void SetSelectCharacter(ulong clientId)
    {
        // Debug.Log("Dropdown Value: " + selectCharacterDropdown.value);
        Debug.Log("Id Value: " + clientId);
        ulong PLAYER_CHAR_ID;
        int playerDataIndex = GameMultiplayerManager.Instance.GetPlayerDataIndexFromClientId(clientId);
        var playerDatasList = GameMultiplayerManager.Instance.GetPlayerDataNetworkList();
        int classId = playerDatasList[playerDataIndex].classId;
        switch (classId)
        {
            case 0:
                PLAYER_CHAR_ID = 1001;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
            case 1:
                PLAYER_CHAR_ID = 1002;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
            case 2:
                PLAYER_CHAR_ID = 1003;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
            default:
                PLAYER_CHAR_ID = 1000;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
        }
        // PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(UserManager.Instance.UserData.PlayerCharacterId);
    }
    protected override void InitAfterAwake()
    {
        selectCharacterButton_Host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        selectCharacterButton_Client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    public override void OnNetworkSpawn()
    {
        // PlayerManager.Instance.OnAfterClientConnect += SetSelectCharacter;

    }



    public override void OnNetworkDespawn()
    {

        // PlayerManager.Instance.OnAfterClientConnect -= SetSelectCharacter;

    }
    public void SetPlayerCrossHairState(bool isActive)
    {
        crossHair.SetActive(isActive);
    }

    public void SetLockTargetPosition(Vector3 worldPos, bool isScreenPos = false)
    {
        if (isScreenPos)
        {
            lockTarget.transform.position = worldPos;
        }
        else
        {
            lockTarget.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
    }
    public void ShowRespawnCountdown()
    {
        isShowRespawnUI = true;
        respawnCountdown = RespawnCountdown;
        respawnCountdownUI.SetActive(true);
    }
    public void HideRespawnCountdown()
    {
        isShowRespawnUI = false;
        respawnCountdownUI.SetActive(false);
    }
    public void ShowResultUI()
    {
        resultUI.SetActive(true);
    }
    public void HideResultUI()
    {
        resultUI.SetActive(false);
    }
    public void SetLockTargetState(bool isActive)
    {
        lockTarget.SetActive(isActive);
    }

    public void FullScreen_Player_Hit()
    {
        fullScreen_Hit.DOKill();
        fullScreen_Hit.DOFloat(FS_Hit_Intensity, "_Intensity", FS_Hit_Duration).OnComplete(() =>
        {
            fullScreen_Hit.DOFloat(0, "_Intensity", FS_Hit_Duration);
        });
    }

    public Tween FadeScreenOut(Color fadeToColor = default)
    {
        fadeScreen.color = fadeToColor;
        return fadeScreen.DOFade(1, fadeDuration);
    }

    public Tween FadeScreenIn(Color fadeFromColor = default)
    {
        fadeScreen.color = fadeFromColor;
        return fadeScreen.DOFade(0, fadeDuration);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        fullScreen_Hit.SetFloat("_Intensity", 0);
    }
}
