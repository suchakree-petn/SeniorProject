using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public bool CanCameraMove { get; private set; }
    [SerializeField] private PlayerCameraMode cameraMode;
    [SerializeField,Range(0,1000)] private float smoothness;

    [Header("Reference")]
    private Transform mainCameraTransform;
    [SerializeField] private Transform FPSCameraTransform;
    [SerializeField] private Transform TPSCameraTransform;


    private void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        CanCameraMove = true;

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
            transform.rotation = Quaternion.Euler(0, mainCameraTransform.rotation.eulerAngles.y, 0f);
        }

    }

}
public enum PlayerCameraMode
{
    FirstPerson = 0,
    ThirdPerson = 1
}
