using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : NetworkSingleton<PlayerUIManager>
{
    [FoldoutGroup("Hit FS Config")][SerializeField] float FS_Hit_Intensity = 0.1f;
    [FoldoutGroup("Hit FS Config")][SerializeField] float FS_Hit_Duration = 0.3f;
    [FoldoutGroup("Hit FS Config")][SerializeField] float fadeDuration = 4;

    [FoldoutGroup("Respawn Config")][SerializeField] float respawnCountdown = 10;
    [FoldoutGroup("Respawn Config")][SerializeField] float _respawnCountdown;
    [FoldoutGroup("Respawn Config")][SerializeField] bool isShowRespawnUI;

    [FoldoutGroup("Reference UI")][SerializeField] GameObject playerCanvas;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject crossHair;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject lockTarget;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject respawnCountdownUI;
    [FoldoutGroup("Reference UI")][SerializeField] TextMeshProUGUI respawnCountdownUI_text;
    [FoldoutGroup("Reference UI")][SerializeField] GameObject resultUI;
    [FoldoutGroup("Reference UI")][SerializeField] Material fullScreen_Hit;
    [FoldoutGroup("Reference UI")][SerializeField] Image fadeScreen;



    private void Update()
    {
        if (isShowRespawnUI)
        {
            _respawnCountdown -= Time.deltaTime;
            respawnCountdownUI_text.text = "Respawn in: " + (int)_respawnCountdown;
        }
    }
    public void SetSelectCharacter(ulong clientId)
    {
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

    }

    public override void OnNetworkSpawn()
    {

    }



    public override void OnNetworkDespawn()
    {


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
        _respawnCountdown = respawnCountdown;
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
