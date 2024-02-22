using System;
using Cinemachine;
using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    [SerializeField] private MouseMovementConfig mouseMovementConfig;
    [SerializeField] private LayerMask FPS_MainCamCullingMask;
    [SerializeField] private LayerMask TPS_MainCamCullingMask;
    public bool CanCameraMove { get; private set; }
    private Vector2 lookInput = new();
    private Vector2 _playerLookInput = new();
    private Vector2 _prevPlayerLookInput = new();
    private float cameraPitch;

    private PlayerActions _playerAction;

    [Header("Reference")]
    [SerializeField] private Transform playerView;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform FPSCameraTransform;
    [SerializeField] private Transform TPSCameraTransform;
    [SerializeField] private Rigidbody rb;
    private Camera mainCam;


    private void Awake()
    {
        _playerAction = new();
        _playerAction.Enable();
        CanCameraMove = true;
        mainCam = Camera.main;
    }
    private void Start()
    {
        LockMouseCursor();
        SetCameraMode(PlayerCameraMode.FirstPerson);
    }

    private static void LockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [Command]
    private void SetCameraMode(PlayerCameraMode newMode)
    {
        Transform overlayCamParent = mainCam.transform.GetChild(0);

        switch (newMode)
        {
            case PlayerCameraMode.FirstPerson:
                FPSCameraTransform.SetParent(null);
                FPSCameraTransform.gameObject.SetActive(true);
                TPSCameraTransform.gameObject.SetActive(false);

                mainCam.cullingMask = FPS_MainCamCullingMask;
                overlayCamParent.gameObject.SetActive(true);

                break;
            case PlayerCameraMode.ThirdPerson:
                FPSCameraTransform.gameObject.SetActive(false);
                TPSCameraTransform.gameObject.SetActive(true);

                mainCam.cullingMask = TPS_MainCamCullingMask;
                overlayCamParent.gameObject.SetActive(false);
                break;
        }
    }

    private void FixedUpdate()
    {

        if (CanCameraMove)
        {
            _playerLookInput = GetLookInput();

            switch (mouseMovementConfig.cameraMode)
            {
                case PlayerCameraMode.FirstPerson:
                    PlayerLook_FPS();
                    CameraLook_FPS();
                    break;
                case PlayerCameraMode.ThirdPerson:
                    PlayerLook_TPS();
                    CameraLook_TPS();
                    break;

            }
        }

    }

    private void PlayerLook_FPS()
    {
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + (_playerLookInput.x * mouseMovementConfig.FPS_X_Axis_Sensitive), 0f);
    }
    private void PlayerLook_TPS()
    {
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + (_playerLookInput.x * mouseMovementConfig.TPS_X_Axis_Sensitive), 0f);
    }
    private void CameraLook_FPS()
    {
        cameraPitch -= _playerLookInput.y * mouseMovementConfig.FPS_Y_Axis_Sensitive;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerView.rotation = Quaternion.Euler(cameraPitch, playerView.rotation.eulerAngles.y, playerView.rotation.eulerAngles.z);
    }
    private void CameraLook_TPS()
    {
        cameraPitch -= _playerLookInput.y * mouseMovementConfig.TPS_Y_Axis_Sensitive;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerView.rotation = Quaternion.Euler(cameraPitch, playerView.rotation.eulerAngles.y, playerView.rotation.eulerAngles.z);
    }

    private Vector2 GetLookInput()
    {
        _prevPlayerLookInput = _playerLookInput;
        _playerLookInput = lookInput;
        return Vector2.Lerp(_prevPlayerLookInput, _playerLookInput * Time.deltaTime, 0.35f);
    }
    private void SetLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    private void OnEnable()
    {
        _playerAction.PlayerCharacter.Look.performed += SetLook;
        _playerAction.PlayerCharacter.Look.canceled += SetLook;
    }
    private void OnDisable()
    {
        _playerAction.PlayerCharacter.Look.performed -= SetLook;
        _playerAction.PlayerCharacter.Look.canceled -= SetLook;
    }

}
public enum PlayerCameraMode
{
    FirstPerson = 0,
    ThirdPerson = 1
}

[Serializable]
public class MouseMovementConfig
{
    [ReadOnlyGUI] public PlayerCameraMode cameraMode = PlayerCameraMode.FirstPerson;
    [Range(1, 100)] public float FPS_X_Axis_Sensitive;
    [Range(1, 100)] public float FPS_Y_Axis_Sensitive;
    [Range(1, 100)] public float TPS_X_Axis_Sensitive;
    [Range(1, 100)] public float TPS_Y_Axis_Sensitive;
}
