using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovementConfig playerMovementConfig;
    public PlayerMovementConfig PlayerMovementConfig => playerMovementConfig;
    [SerializeField, ReadOnlyGUI] private bool isGrounded;
    [SerializeField, ReadOnlyGUI] private bool isOnSlope;
    [SerializeField, ReadOnlyGUI] private bool isRunning;
    private bool IsDyinng => playerController.IsPlayerDie;
    [ReadOnlyGUI] public bool CanMove;
    [SerializeField, ReadOnlyGUI] private Vector3 moveDirection = Vector3.zero;
    public Rigidbody PlayerRigidbody => playerRb;

    private PlayerCameraMode playerCameraMode;
    private RaycastHit slopeHit;

    [Header("Reference")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerController playerController;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // Draw a wireframe sphere at the object's position with the specified radius
        Gizmos.DrawWireSphere(groundCheck.position,
            playerMovementConfig.groundDistance);

        // Draw a line that check isOnSlope
        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * playerMovementConfig.groundDistance, Color.blue);
    }
    void Update()
    {
        MoveSpeedCalc();
        CheckGround();
        CheckSlope();
        DragProcess();
        MovementProcess();
        LimitVelocity();
    }
    private void FixedUpdate()
    {
        // ApplyGravity();
    }
    public void ApplyGravity()
    {
        if (!isGrounded && !isOnSlope)
        {
            playerRb.AddForce(-Physics.gravity.y * playerMovementConfig.gravityMultiplier * Vector3.down, ForceMode.Force);
        }
    }

    public Vector3 GetMovementForce()
    {
        Rigidbody rb = PlayerRigidbody;
        Vector3 initialVelocity = rb.velocity;
        float mass = rb.mass;
        Vector3 force = moveDirection;
        Vector3 acceleration = force / mass;
        float timeInterval = Time.fixedDeltaTime;
        Vector3 velocityChange = acceleration * timeInterval;
        Vector3 finalVelocity = initialVelocity + velocityChange;
        return finalVelocity;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            playerMovementConfig.groundDistance,
            playerMovementConfig.groundMask);
    }
    private void CheckSlope()
    {
        if (Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            out slopeHit,
            playerMovementConfig.groundDistance,
            playerMovementConfig.groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            isOnSlope = angle < playerMovementConfig.maxSlope && angle != 0 && isGrounded;
            return;
        }
        isOnSlope = false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
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
        InputAction movementAction = PlayerInputManager.Instance.MovementAction;
        Vector2 movementInput = movementAction.ReadValue<Vector2>();
        Transform mainCamTransform = Camera.main.transform;
        switch (playerCameraMode)
        {
            case PlayerCameraMode.ThirdPerson:
                moveDirection = ((mainCamTransform.forward * movementInput.y) + (mainCamTransform.right * movementInput.x)).normalized;
                if (isOnSlope)
                    moveDirection = GetSlopeMoveDirection();
                else
                    moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;
                break;
            case PlayerCameraMode.Focus:
                Vector3 camForward = Vector3.ProjectOnPlane(mainCamTransform.forward, Vector3.up).normalized;
                moveDirection = ((camForward * movementInput.y) + (mainCamTransform.right * movementInput.x)).normalized;
                if (isOnSlope)
                    moveDirection = GetSlopeMoveDirection();
                break;
        }
    }

    public void SetCameraMode(PlayerCameraMode playerCameraMode)
    {
        this.playerCameraMode = playerCameraMode;

    }
    public void MoveCharactorThirdPerson()
    {
        InputAction movementAction = PlayerInputManager.Instance.MovementAction;
        if (movementAction.IsPressed() && playerCameraMode == PlayerCameraMode.ThirdPerson)
        {
            Vector3 moveDirOnPlane = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirOnPlane, Vector3.up);
            playerRb.rotation = Quaternion.RotateTowards(playerRb.rotation, targetRotation, 500 * Time.deltaTime);
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

        if (IsDyinng)
        {
            playerMovementConfig.MovementPenaltyPercentage += playerMovementConfig.DyingMovingPenaltyPercentage;
        }

        float ratio = 1 + (playerMovementConfig.MovementBonusPercentage - playerMovementConfig.MovementPenaltyPercentage) / 100f;
        playerMovementConfig.MoveSpeed = playerMovementConfig._defaultMoveSpeed * ratio;
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && CanMove && !IsDyinng)
        {
            Debug.Log(playerRb.GetInstanceID());
            playerRb.velocity = new(playerRb.velocity.x, 0, playerRb.velocity.z);
            playerRb.AddForce(transform.up * playerMovementConfig.jumpPower, ForceMode.Impulse);

        }
    }

    public void PlayerRun(InputAction.CallbackContext context)
    {
        if (!isRunning && !IsDyinng)
        {
            isRunning = true;
        }
    }
    public void PlayerStopRun(InputAction.CallbackContext context)
    {
        if (isRunning || IsDyinng)
        {
            isRunning = false;
        }
    }

    public Vector3 GetMoveSpeedRatioOfMaxMoveSpeed(Vector3 moveVelocity)
    {
        float ratioMoveBonus = 1 + playerMovementConfig.RunSpeedBonusPercentage / 100f;
        float maxMoveSpeed = playerMovementConfig._defaultMoveSpeed * ratioMoveBonus;
        Vector3 ratio = new(moveVelocity.x / maxMoveSpeed, moveVelocity.y / maxMoveSpeed, moveVelocity.z / maxMoveSpeed);
        return ratio;
    }
    public Vector3 GetMoveSpeedRatioOfNormalMoveSpeed(Vector3 moveVelocity)
    {
        float maxMoveSpeed = playerMovementConfig._defaultMoveSpeed;
        Vector3 ratio = new(moveVelocity.x / maxMoveSpeed, moveVelocity.y / maxMoveSpeed, moveVelocity.z / maxMoveSpeed);
        return ratio;
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        playerMovementConfig._defaultMoveSpeed = playerMovementConfig.MoveSpeed;

    }
#endif
}

[Serializable]
public class PlayerMovementConfig
{
    [HideInInspector] public float _defaultMoveSpeed;
    public float MoveSpeed = 6f;
    public float jumpPower = 7f;
    [Range(0, 4)] public float groundDistance = 0.4f;
    [Range(0, 10)] public float groundDrag = 0f;
    [Range(1, 10)] public float gravityMultiplier = 1f;
    [Range(0.01f, 90f)] public float maxSlope = 45f;
    public LayerMask groundMask;

    [Header("Speed Bonus & Penalty")]
    [Range(0, 100)] public float RunSpeedBonusPercentage;
    [Range(0, 100)] public float InAirPenaltyPercentage;
    [Range(0, 100)] public float DyingMovingPenaltyPercentage;

    [ReadOnlyGUI] public float MovementBonusPercentage;
    [ReadOnlyGUI] public float MovementPenaltyPercentage;

    public PlayerMovementConfig()
    {
        _defaultMoveSpeed = MoveSpeed;
    }

}
