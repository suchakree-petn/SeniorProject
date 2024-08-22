using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PingMenuManager : NetworkSingleton<PingMenuManager>
{
    [SerializeField] private GameObject pingMenuRootObject;
    [SerializeField] private Transform CursorRootObject;
    [SerializeField] private Transform HighlightRootObject;
    [SerializeField] private List<Sprite> listPingSprite;
    [SerializeField] private List<string> listdetail;
    [SerializeField] private Image pingImage;
    [SerializeField] float inCircle = 20;
    [SerializeField] float sphereRadian;
    [SerializeField] private GameObject pingCursor, pingHighlight;
    public Transform PlayerCamera
    {
        get
        {
            return Camera.main.transform;
        }
    }
    [SerializeField] private LayerMask raycastableLayers;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject pingObjectPrefab;
    [SerializeField] private GameObject pingObjectParent;
    [SerializeField] private GameObject cancelTextGameObject;
    [SerializeField] string toggleTagName = "TogglePing";
    [SerializeField] public Toggle ToggleActive{
        get
        {
            if(!toggle){
                toggle = GameObject.FindGameObjectWithTag(toggleTagName).GetComponent<Toggle>();
            }
            return toggle;
        }
    }
    Toggle toggle;

    float freeXSpeed = 0, freeYSpeed = 0, focusXSpeed = 0, focusYSpeed = 0, delayShowMenu;
    int selectedItem;
    bool isMouseMove, isUsePing;
    protected override void InitAfterAwake()
    {
    }
    void Start()
    {
        pingMenuRootObject.SetActive(false);
    }

    void Update()
    {
        if (PlayerManager.Instance != null)
        {
            if (Input.GetMouseButtonDown(2))
            {
                selectedItem = 6;
                delayShowMenu = 0;
                isMouseMove = false;
                isUsePing = true;

                SetDefaultPointer();
                MouseCanMove(false);
            }
            if (Input.GetMouseButtonUp(2) && isUsePing)
            {
                pingMenuRootObject.SetActive(false);
                MouseCanMove(true);

                RaycastHit hit;

                if (Physics.SphereCast(PlayerCamera.position, sphereRadian, PlayerCamera.TransformDirection(Vector3.forward), out hit, 1000f, raycastableLayers))
                {
                    DeletePingInScene();
                    if (0 != (enemyLayers.value & 1 << hit.collider.gameObject.layer) & !isMouseMove)
                    {
                        selectedItem = 7;
                    }
                    CreatePingManagerServerRpc(hit.point, NetworkManager.LocalClientId);
                }
            }
            if (Input.GetMouseButton(2))
            {
                delayShowMenu += Time.deltaTime;
                if (delayShowMenu > 0.3 && isUsePing)
                {
                    pingMenuRootObject.SetActive(true);
                    cancelTextGameObject.SetActive(true);
                    Cursor.visible = true;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    pingMenuRootObject.SetActive(false);
                    MouseCanMove(true);
                    DeletePingInScene();
                    isUsePing = false;
                }
                if (isMouseMove)
                {
                    cancelTextGameObject.SetActive(false);
                    Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                    Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    Vector2 mouseDelta = mousePosition - screenCenter;

                    if (Math.Abs(mouseDelta.x) > inCircle && Math.Abs(mouseDelta.x) > inCircle)
                    {
                        float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;

                        float smoothAngle = Mathf.MoveTowardsAngle(CursorRootObject.eulerAngles.z, angle, mouseDelta.magnitude * Time.deltaTime * 250) - 60;
                        CursorRootObject.transform.eulerAngles = new Vector3(0, 0, smoothAngle);

                        float highlightAngle = Mathf.Round(smoothAngle / 60) * 60;
                        HighlightRootObject.transform.eulerAngles = new Vector3(0, 0, highlightAngle);

                        int pointerItem = (int)Mathf.Round(smoothAngle / 60);
                        // selectedItem = pointerItem % 6;


                        if (pointerItem <= 0)
                        {
                            selectedItem = Math.Abs(pointerItem);
                        }
                        else
                        {
                            selectedItem = Math.Abs(6 - pointerItem);
                        }

                        pingImage.sprite = listPingSprite[selectedItem];
                    }
                }
                else
                {
                    Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                    Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    Vector2 mouseDelta = mousePosition - screenCenter;

                    if (mouseDelta.magnitude > inCircle)
                    {
                        pingCursor.SetActive(true);
                        pingHighlight.SetActive(true);
                        pingImage.gameObject.SetActive(true);
                        isMouseMove = true;
                    }
                }
            }
        }
    }
    void CreatePing(Vector3 position, ulong clientId, int classId)
    {
        GameObject pingGameObject = Instantiate(pingObjectPrefab, pingObjectParent.transform);
        pingGameObject.GetComponent<PingObjectUI>().SetPingUI(listPingSprite[selectedItem], clientId, classId, listdetail[selectedItem]);
        pingGameObject.transform.position = position;
    }
    [ServerRpc(RequireOwnership = false)]
    void CreatePingManagerServerRpc(Vector3 position, ulong clientId)
    {
        PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromClientId(clientId);
        CreatePingMessageClientRpc(position, clientId, playerData.classId);
    }
    [ClientRpc]
    void CreatePingMessageClientRpc(Vector3 position, ulong clientId, int classId)
    {
        CreatePing(position, clientId, classId);
    }

    void DeletePing(int index)
    {
        Destroy(pingObjectParent.transform.GetChild(index).gameObject);
    }
    [ServerRpc(RequireOwnership = false)]
    void DeletePingManagerServerRpc(int index)
    {
        DeletePingMessageClientRpc(index);
    }
    [ClientRpc]
    void DeletePingMessageClientRpc(int index)
    {
        DeletePing(index);
    }
    public void DeletePingInScene()
    {
        if (pingObjectParent.transform.childCount > 0)
        {
            for (int i = 0; i < pingObjectParent.transform.childCount; i++)
            {
                GameObject pingObject = pingObjectParent.transform.GetChild(i).gameObject;
                if (pingObject.GetComponent<PingObjectUI>().GetIDPing() == NetworkManager.LocalClientId)
                {
                    DeletePingManagerServerRpc(i);
                }
            }
        }
    }
    void SetDefaultPointer()
    {
        pingCursor.SetActive(false);
        pingHighlight.SetActive(false);
        pingImage.gameObject.SetActive(false);
    }
    void MouseCanMove(bool mouseCanMove)
    {
        if (mouseCanMove)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = freeXSpeed;
            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = freeYSpeed;
            PlayerManager.Instance.LocalPlayerController.GetMouseMovement().CanCameraMove = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;

            freeXSpeed = CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed;
            freeYSpeed = CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed;

            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = 0;
            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 0;
            PlayerManager.Instance.LocalPlayerController.GetMouseMovement().CanCameraMove = false;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ActivePingServerRpc(bool active)
    {
        ActivePingClientRpc(active);
    }
    [ClientRpc]
    void ActivePingClientRpc(bool active)
    {
        ToggleActive.isOn = active;
        // PingMenuManager.Instance.gameObject.SetActive(active);
    }

}
