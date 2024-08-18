using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PingMenuManager : NetworkSingleton<PingMenuManager>
{
    [SerializeField] private GameObject pingMenuRootObject;
    [SerializeField] private Transform CursorRootObject;
    [SerializeField] private Transform HighlightRootObject;
    [SerializeField] private List<Sprite> listPingSprite;
    [SerializeField] private Image pingImage;
    [SerializeField] float inCircle = 20;
    [SerializeField] private GameObject pingCursor, pingHighlight;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private LayerMask raycastableLayers;
    [SerializeField] private GameObject pingObjectPrefab;
    [SerializeField] private GameObject pingObjectParent;

    float xSpeed = 0, ySpeed = 0;
    int selectedItem;
    bool isMouseMove;
    protected override void InitAfterAwake()
    {
    }
    void Start()
    {
        pingMenuRootObject.SetActive(false);
        playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            selectedItem = -1;
            isMouseMove = false;
            pingMenuRootObject.SetActive(true);
            SetDefaultPointer();
            MouseCanMove(false);


        }
        if (Input.GetMouseButtonUp(2))
        {
            pingMenuRootObject.SetActive(false);
            MouseCanMove(true);

            if (selectedItem != -1)
            {
                RaycastHit hit;

                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out hit))
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
                    CreatePingManagerServerRpc(hit.point, NetworkManager.LocalClientId);
                }
            }
        }
        if (Input.GetMouseButton(2))
        {
            if (isMouseMove)
            {
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
    void CreatePing(Vector3 position, ulong clientId)
    {
        GameObject pingGameObject = Instantiate(pingObjectPrefab, pingObjectParent.transform);
        pingGameObject.GetComponent<PingObjectUI>().SetPingUI(listPingSprite[selectedItem], clientId);
        pingGameObject.transform.position = position;
    }
    [ServerRpc(RequireOwnership = false)]
    void CreatePingManagerServerRpc(Vector3 position, ulong clientId)
    {
        Debug.Log("CreatePingManagerServerRpc");
        CreatePingMessageClientRpc(position, clientId);
    }
    [ClientRpc]
    void CreatePingMessageClientRpc(Vector3 position, ulong clientId)
    {
        // Vector3 position = new Vector3(x, y, z);
        CreatePing(position, clientId);
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

            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = xSpeed;
            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = ySpeed;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            xSpeed = CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed;
            ySpeed = CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed;

            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = 0;
            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 0;
        }
    }


}
