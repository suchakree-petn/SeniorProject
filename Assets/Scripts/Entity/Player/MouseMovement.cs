using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    public bool CanCameraMove { get; private set; }
    [SerializeField] private PlayerCameraMode cameraMode;
    private Vector2 lookInput = new();
    private Vector2 _playerLookInput = new();
    private Vector2 _prevPlayerLookInput = new();
    private float cameraPitch;
    [SerializeField] private float FPS_X_Axis_Sensitive;
    [SerializeField] private float FPS_Y_Axis_Sensitive;
    [SerializeField] private float TPS_X_Axis_Sensitive;
    [SerializeField] private float TPS_Y_Axis_Sensitive;

    private PlayerActions _playerAction;

    [Header("Reference")]
    [SerializeField] private Transform playerView;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Transform FPSCameraTransform;
    [SerializeField] private Transform TPSCameraTransform;
    [SerializeField] private Rigidbody rb;


    private void Awake()
    {
        _playerAction = new();
        _playerAction.Enable();
        CanCameraMove = true;
        mainCamera = Camera.main;
    }
    private void Start()
    {
        LockMouseCursor();
        InitCameraMode();
    }

    private static void LockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitCameraMode()
    {
        switch (cameraMode)
        {
            case PlayerCameraMode.FirstPerson:
                FPSCameraTransform.SetParent(null);
                FPSCameraTransform.gameObject.SetActive(true);
                TPSCameraTransform.gameObject.SetActive(false);
                break;
            case PlayerCameraMode.ThirdPerson:
                FPSCameraTransform.gameObject.SetActive(false);
                TPSCameraTransform.gameObject.SetActive(true);
                break;
        }
    }

    private void FixedUpdate()
    {

        if (CanCameraMove)
        {
            _playerLookInput = GetLookInput();

            switch (cameraMode)
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
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + (_playerLookInput.x * FPS_X_Axis_Sensitive), 0f);
    }
    private void PlayerLook_TPS()
    {
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + (_playerLookInput.x * TPS_X_Axis_Sensitive), 0f);
    }
    private void CameraLook_FPS()
    {
        cameraPitch -= _playerLookInput.y * FPS_Y_Axis_Sensitive;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerView.rotation = Quaternion.Euler(cameraPitch, playerView.rotation.eulerAngles.y, playerView.rotation.eulerAngles.z);
    }
    private void CameraLook_TPS()
    {
        cameraPitch -= _playerLookInput.y * TPS_Y_Axis_Sensitive;
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
