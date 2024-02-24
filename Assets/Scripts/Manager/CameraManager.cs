using Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Reference")]
    [SerializeField] private CinemachineFreeLook thirdPersonVCam;
    [SerializeField] private CinemachineVirtualCamera focusVCam;
    protected override void InitAfterAwake()
    {
    }

    public CinemachineFreeLook GetThirdPersonCamera()
    {
        return thirdPersonVCam;
    }
    public CinemachineVirtualCamera GetFocusCamera()
    {
        return focusVCam;
    }
}
