using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public bool CanCameraMove { get; private set; }
    [SerializeField] private PlayerCameraMode cameraMode;
    [SerializeField, Range(0, 1000)] private float smoothness;

    [Header("Reference")]
    private Transform mainCameraTransform;
    [SerializeField] private CinemachineVirtualCamera vcam;
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
            transform.rotation = Quaternion.Euler(0, mainCameraTransform.rotation.eulerAngles.y, 0);
        }

    }
    // public bool CanCameraMove { get; private set; }
    // [SerializeField] private PlayerCameraMode cameraMode;
    // [SerializeField, Range(0, 1000)] private float smoothness;
    // private float xRotation;
    // private float yRotation;

    // [Header("Reference")]
    // private Transform mainCameraTransform;
    // [SerializeField] private Transform FPSCameraTransform;
    // [SerializeField] private Transform TPSCameraTransform;


    // private void Awake()
    // {
    //     mainCameraTransform = Camera.main.transform;
    //     CanCameraMove = true;

    // }
    // private void Start()
    // {
    //     LockMouseCursor();
    //     // InitCameraMode();
    // }

    // private static void LockMouseCursor()
    // {
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    // }

    // private void InitCameraMode()
    // {
    //     switch (cameraMode)
    //     {
    //         case PlayerCameraMode.FirstPerson:
    //             FPSCameraTransform.SetParent(null);
    //             FPSCameraTransform.gameObject.SetActive(true);
    //             TPSCameraTransform.gameObject.SetActive(false);
    //             break;
    //         case PlayerCameraMode.ThirdPerson:
    //             FPSCameraTransform.gameObject.SetActive(false);
    //             TPSCameraTransform.gameObject.SetActive(true);
    //             break;
    //     }
    // }

    // private void FixedUpdate()
    // {
    //     float inputX = Input.GetAxisRaw("Mouse X") * smoothness * Time.fixedDeltaTime;
    //     float inputY = Input.GetAxisRaw("Mouse Y") * smoothness * Time.fixedDeltaTime;
    //     yRotation += inputX;
    //     xRotation -= inputY;
    //     xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    //     Vector3 camPos = transform.position;
    //     mainCameraTransform.SetPositionAndRotation(new(camPos.x,camPos.y+1,camPos.z), Quaternion.Euler(xRotation, yRotation, 0));
    //     transform.rotation = Quaternion.Euler(0, yRotation, 0);
    // }

}
public enum PlayerCameraMode
{
    FirstPerson = 0,
    ThirdPerson = 1
}
