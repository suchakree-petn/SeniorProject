using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovementConfig playerMovementConfig;
    [SerializeField, ReadOnlyGUI] private bool isGrounded;
    [SerializeField, ReadOnlyGUI] private bool isRunning;
    [ReadOnlyGUI] public bool CanMove;
    [SerializeField, ReadOnlyGUI] private Vector3 moveDirection = Vector3.zero;
    private PlayerActions playerActions;
    public InputAction movementAction;
    public InputAction jumpAction;
    public InputAction runAction;
    private PlayerCameraMode playerCameraMode;

    [Header("Reference")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Rigidbody playerRb;



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Draw a wireframe sphere at the object's position with the specified radius
        Gizmos.DrawWireSphere(groundCheck.position,
            playerMovementConfig.groundDistance);
    }
    void Update()
    {
        MoveSpeedCalc();
        CheckGround();
        DragProcess();
        MovementProcess();
        LimitVelocity();
    }
    private void FixedUpdate()
    {
        ApplyGravity();

    }
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            playerRb.AddForce(-Physics.gravity.y * playerMovementConfig.gravityMultiplier * Vector3.down, ForceMode.Force);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            playerMovementConfig.groundDistance,
            playerMovementConfig.groundMask);
    }

    private void DragProcess()
    {
        if (!isGrounded)
        {
            playerRb.drag = 0;
        }
        else
        {
            playerRb.drag = playerMovementConfig.groundDrag;
        }
    }

    private void MovementProcess()
    {
        Vector2 movementInput = movementAction.ReadValue<Vector2>();
        switch (playerCameraMode)
        {
            case PlayerCameraMode.ThirdPerson:
                Transform mainCamTransform = Camera.main.transform;
                moveDirection = ((mainCamTransform.forward * movementInput.y) + (mainCamTransform.right * movementInput.x)).normalized;
                moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;
                break;
            case PlayerCameraMode.Focus:
                moveDirection = ((transform.forward * movementInput.y) + (transform.right * movementInput.x)).normalized;
                break;
        }
    }

    public void InitPlayerActions()
    {
        playerActions = new();
        playerActions.PlayerCharacter.Enable();
        movementAction = playerActions.PlayerCharacter.Movement;
        jumpAction = playerActions.PlayerCharacter.Jump;
        runAction = playerActions.PlayerCharacter.Run;
    }
    public void SetCameraMode(PlayerCameraMode playerCameraMode)
    {
        this.playerCameraMode = playerCameraMode;

    }
    public void MoveCharactorThirdPerson()
    {
        if (movementAction.IsPressed())
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,500 * Time.deltaTime);
        }

        playerRb.AddForce(10 * playerMovementConfig.MoveSpeed * moveDirection, ForceMode.Force);
    }
    public void MoveCharactorFocus()
    {

        playerRb.AddForce(10 * playerMovementConfig.MoveSpeed * moveDirection, ForceMode.Force);
    }
    public void MoveCharactor()
    {
        if (CanMove)
        {
            switch (playerCameraMode)
            {
                case PlayerCameraMode.ThirdPerson:
                    MoveCharactorThirdPerson();
                    break;
                case PlayerCameraMode.Focus:
                    MoveCharactorFocus();
                    break;
            }
        }

    }

    private void LimitVelocity()
    {
        Vector3 horizontalVelocity = new(playerRb.velocity.x, 0, playerRb.velocity.z);

        if (horizontalVelocity.magnitude > playerMovementConfig.MoveSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * playerMovementConfig.MoveSpeed;
            playerRb.velocity = new(limitedVelocity.x, playerRb.velocity.y, limitedVelocity.z);
        }
    }
    private void MoveSpeedCalc()
    {
        playerMovementConfig.MovementBonusPercentage = 0;
        playerMovementConfig.MovementPenaltyPercentage = 0;

        if (isRunning)
        {
            playerMovementConfig.MovementBonusPercentage += playerMovementConfig.RunSpeedBonusPercentage;
        }

        if (!isGrounded)
        {
            playerMovementConfig.MovementPenaltyPercentage += playerMovementConfig.InAirPenaltyPercentage;
        }

        float ratio = 1 + (playerMovementConfig.MovementBonusPercentage - playerMovementConfig.MovementPenaltyPercentage) / 100f;
        playerMovementConfig.MoveSpeed = playerMovementConfig._defaultWalkSpeed * ratio;
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && CanMove)
        {
            playerRb.velocity = new(playerRb.velocity.x, 0, playerRb.velocity.z);
            playerRb.AddForce(transform.up * playerMovementConfig.jumpPower, ForceMode.Impulse);

        }
    }

    public void PlayerRun(InputAction.CallbackContext context)
    {
        if (!isRunning && Input.GetKey(KeyCode.W))
        {
            isRunning = true;
        }
    }
    public void PlayerStopRun(InputAction.CallbackContext context)
    {
        if (isRunning && !Input.GetKey(KeyCode.W))
        {
            isRunning = false;
        }
    }



#if UNITY_EDITOR
    public void OnValidate()
    {
        playerMovementConfig._defaultWalkSpeed = playerMovementConfig.MoveSpeed;

    }
#endif
}

[Serializable]
public class PlayerMovementConfig
{
    [HideInInspector] public float _defaultWalkSpeed;
    public float MoveSpeed = 6f;
    public float jumpPower = 7f;
    [Range(0, 4)] public float groundDistance = 0.4f;
    [Range(0, 10)] public float groundDrag = 0f;
    [Range(1, 3)] public float gravityMultiplier = 1f;
    public LayerMask groundMask;

    [Header("Speed Bonus & Penalty")]
    [Range(0, 100)] public float RunSpeedBonusPercentage;
    [Range(0, 100)] public float InAirPenaltyPercentage;

    [ReadOnlyGUI] public float MovementBonusPercentage;
    [ReadOnlyGUI] public float MovementPenaltyPercentage;

    public PlayerMovementConfig()
    {
        _defaultWalkSpeed = MoveSpeed;
    }

}
