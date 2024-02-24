using System;
using Cinemachine;
using QFSW.QC;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    [SerializeField] private MouseMovementConfig mouseMovementConfig;
    public bool CanCameraMove = true;
    [SerializeField] private Vector2 lookInput = new();
    private Vector2 _playerLookInput = new();
    private Vector2 _prevPlayerLookInput = new();
    private float cameraPitch;
    public PlayerActions playerActions;
    private PlayerCameraMode playerCameraMode;

    [Header("Reference")]
    [SerializeField] private Transform playerView;
    [SerializeField] private Rigidbody rb;
    private Transform FocusCameraTransform;
    private Transform ThirdPersonCameraTransform;
    private Camera mainCam;



    public void InitCameras(PlayerCameraMode playerCameraMode)
    {
        mainCam = Camera.main;

        CinemachineVirtualCamera FocusVCam = CameraManager.Instance.GetFocusCamera();
        FocusVCam.Follow = playerView;
        FocusCameraTransform = FocusVCam.transform;

        CinemachineFreeLook thirdPersonVCam = CameraManager.Instance.GetThirdPersonCamera();
        thirdPersonVCam.Follow = transform;
        thirdPersonVCam.LookAt = transform;
        ThirdPersonCameraTransform = thirdPersonVCam.transform;

        this.playerCameraMode = playerCameraMode;

    }
    public void Init(PlayerCameraMode playerCameraMode)
    {
        this.playerCameraMode = playerCameraMode;

    }


    public void InitPlayerActions()
    {
        playerActions = new();
        playerActions.PlayerCharacter.Enable();
    }
    public void LockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [Command]
    public void SetCameraMode(PlayerCameraMode newMode, bool isShowCrossHair = true)
    {
        switch (newMode)
        {
            case PlayerCameraMode.ThirdPerson:
                SetThirdperson(isShowCrossHair);
                break;
            case PlayerCameraMode.Focus:
                SetFocus(isShowCrossHair);
                break;
        }
    }

    private void SetFocus(bool isShowCrossHair = true)
    {
        // FocusCameraTransform.SetParent(null);
        FocusCameraTransform.gameObject.SetActive(true);
        ThirdPersonCameraTransform.gameObject.SetActive(false);
        PlayerUIManager.Instance.SetPlayerCrossHairState(isShowCrossHair);
    }
    private void SetThirdperson(bool isShowCrossHair = false)
    {
        FocusCameraTransform.gameObject.SetActive(false);
        ThirdPersonCameraTransform.gameObject.SetActive(true);
        PlayerUIManager.Instance.SetPlayerCrossHairState(isShowCrossHair);
    }


    public void RotateCamera()
    {
        if (CanCameraMove)
        {
            _playerLookInput = GetLookInput();

            switch (playerCameraMode)
            {
                case PlayerCameraMode.Focus:
                    PlayerLook_Focus();
                    CameraLook_FPS();
                    break;
                case PlayerCameraMode.ThirdPerson:
                    PlayerLook_ThirdPerson();
                    CameraLook_TPS();
                    break;
            }
        }
    }

    private void PlayerLook_Focus()
    {
        rb.rotation = Quaternion.Euler(0f, rb.rotation.eulerAngles.y + (_playerLookInput.x * mouseMovementConfig.FPS_X_Axis_Sensitive), 0f);
    }
    private void PlayerLook_ThirdPerson()
    {
        // CinemachineFreeLook thirdPersonCam = CameraManager.Instance.GetThirdPersonCamera();
        // float inputXValue = thirdPersonCam.m_XAxis.m_InputAxisValue;
        // float inputYValue = thirdPersonCam.m_YAxis.m_InputAxisValue;
        // if (playerActions.PlayerCharacter.Movement.IsPressed())
        // {
        //     float rotationAngle = Mathf.Atan2(inputXValue, inputYValue) * Mathf.Rad2Deg ;
        //     Debug.Log(rotationAngle);
        //     Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);

        //     transform.rotation = rotation;
        // }

    }
    private void CameraLook_FPS()
    {
        cameraPitch -= _playerLookInput.y * mouseMovementConfig.FPS_Y_Axis_Sensitive;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerView.rotation = Quaternion.Euler(cameraPitch, playerView.rotation.eulerAngles.y, playerView.rotation.eulerAngles.z);
    }
    private void CameraLook_TPS()
    {
        // cameraPitch -= _playerLookInput.y * mouseMovementConfig.TPS_Y_Axis_Sensitive;
        // cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        // playerView.rotation = Quaternion.Euler(cameraPitch, playerView.rotation.eulerAngles.y, playerView.rotation.eulerAngles.z);
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

}
public enum PlayerCameraMode
{
    ThirdPerson = 0,
    Focus = 1
}

[Serializable]
public class MouseMovementConfig
{
    [Range(1, 100)] public float FPS_X_Axis_Sensitive;
    [Range(1, 100)] public float FPS_Y_Axis_Sensitive;
    [Range(1, 100)] public float TPS_X_Axis_Sensitive;
    [Range(1, 100)] public float TPS_Y_Axis_Sensitive;
}
