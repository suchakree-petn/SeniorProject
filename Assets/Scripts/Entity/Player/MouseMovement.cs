using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    [SerializeField] private MouseMovementConfig mouseMovementConfig;
    public bool CanCameraMove = true;
    [SerializeField] private Vector2 lookInput = new();
    [SerializeField] private float rotateOffset;
    private Vector2 _playerLookInput = new();
    private Vector2 _prevPlayerLookInput = new();
    private float cameraPitch;
    private PlayerCameraMode playerCameraMode;

    [Header("Reference")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform thirdPersonView;
    [SerializeField] private Transform focusView;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform characterModel;
    private Camera mainCam;


    public void InitCameras(PlayerCameraMode playerCameraMode)
    {
        mainCam = Camera.main;

        CinemachineVirtualCamera FocusVCam = CameraManager.Instance.GetFocusCamera();
        FocusVCam.Follow = focusView;
        FocusVCam.gameObject.SetActive(true);

        CinemachineFreeLook thirdPersonVCam = CameraManager.Instance.GetThirdPersonCamera();
        thirdPersonVCam.Follow = playerTransform;
        thirdPersonVCam.LookAt = thirdPersonView;
        thirdPersonVCam.gameObject.SetActive(true);

        this.playerCameraMode = playerCameraMode;
    }
    public void SetCameraMode(PlayerCameraMode playerCameraMode)
    {
        this.playerCameraMode = playerCameraMode;
    }

    public void LockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetFocus(bool isShowCrossHair = true)
    {
        CameraManager.Instance.GetThirdPersonCamera().Priority = 0;
        CameraManager.Instance.GetFocusCamera().Priority = 10;
        playerTransform.rotation = Quaternion.Euler(0f, mainCam.transform.rotation.eulerAngles.y, 0f);
        PlayerUIManager.Instance.SetPlayerCrossHairState(isShowCrossHair);
        characterModel.localRotation = Quaternion.Euler(0, characterModel.rotation.y + rotateOffset, 0);
    }
    public void SetThirdperson(bool isShowCrossHair = false)
    {
        CameraManager.Instance.GetThirdPersonCamera().m_XAxis.Value = 360;
        CameraManager.Instance.GetThirdPersonCamera().Priority = 10;
        CameraManager.Instance.GetFocusCamera().Priority = 0;
        PlayerUIManager.Instance.SetPlayerCrossHairState(isShowCrossHair);
        characterModel.localRotation = Quaternion.Euler(Vector3.zero);

    }

    public void RotateCamera()
    {

        if (CanCameraMove)
        {
            _playerLookInput = GetLookInput();

            switch (playerCameraMode)
            {
                case PlayerCameraMode.Focus:
                    CameraLook_FPS();
                    break;
                case PlayerCameraMode.ThirdPerson:
                    CameraLook_TPS();
                    break;
                default:
                    Debug.LogWarning("Warning: CameraMode");
                    break;
            }
        }
    }
    public void RotatePlayer()
    {

        if (CanCameraMove)
        {
            _playerLookInput = GetLookInput();

            switch (playerCameraMode)
            {
                case PlayerCameraMode.Focus:
                    PlayerLook_Focus();
                    break;
                case PlayerCameraMode.ThirdPerson:
                    PlayerLook_ThirdPerson();
                    break;
                default:
                    Debug.LogWarning("Warning: CameraMode");
                    break;
            }
        }
    }

    public void RotatePlayer_Update()
    {

        if (CanCameraMove)
        {
            _playerLookInput = GetLookInput();

            switch (playerCameraMode)
            {
                case PlayerCameraMode.Focus:
                    PlayerLook_Focus_Update();
                    break;
                case PlayerCameraMode.ThirdPerson:
                    // PlayerLook_ThirdPerson();
                    break;
                default:
                    Debug.LogWarning("Warning: CameraMode");
                    break;
            }
        }
    }
    private void PlayerLook_Focus()
    {
        rb.transform.rotation = Quaternion.Euler(0f, rb.transform.rotation.eulerAngles.y + (_playerLookInput.x * mouseMovementConfig.Focus_X_Axis_Sensitive), 0f);
    }
    private void PlayerLook_Focus_Update()
    {
        Quaternion rotate = Quaternion.Euler(0f, rb.transform.rotation.eulerAngles.y + (_playerLookInput.x * mouseMovementConfig.Focus_X_Axis_Sensitive), 0f);
        rb.transform.rotation = rotate;
    }
    private void PlayerLook_ThirdPerson()
    {

    }
    private void CameraLook_FPS()
    {
        cameraPitch -= _playerLookInput.y * mouseMovementConfig.Focus_Y_Axis_Sensitive;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        focusView.rotation = Quaternion.Euler(cameraPitch, focusView.rotation.eulerAngles.y, focusView.rotation.eulerAngles.z);
    }
    private void CameraLook_TPS()
    {

    }

    private Vector2 GetLookInput()
    {
        _prevPlayerLookInput = _playerLookInput;
        _playerLookInput = lookInput;
        return Vector2.Lerp(_prevPlayerLookInput, _playerLookInput * Time.deltaTime, 0.35f);
    }
    public void SetLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
#if UNITY_EDITOR
    public void OnValidate()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.GetThirdPersonCamera().m_XAxis.m_MaxSpeed = mouseMovementConfig.ThirdPerson_X_Axis_Sensitive;
            CameraManager.Instance.GetThirdPersonCamera().m_YAxis.m_MaxSpeed = mouseMovementConfig.ThirdPerson_Y_Axis_Sensitive;
        }


    }
#endif
}
public enum PlayerCameraMode
{
    ThirdPerson = 0,
    Focus = 1
}

[Serializable]
public class MouseMovementConfig
{
    [Range(0.1f, 100f)] public float Focus_X_Axis_Sensitive;
    [Range(0.1f, 100f)] public float Focus_Y_Axis_Sensitive;
    [Range(0.1f, 100f)] public float ThirdPerson_X_Axis_Sensitive;
    [Range(0.1f, 100f)] public float ThirdPerson_Y_Axis_Sensitive;
}
