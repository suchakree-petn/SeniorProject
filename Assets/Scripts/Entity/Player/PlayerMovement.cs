using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovementConfig playerMovementConfig;
    [SerializeField, ReadOnlyGUI] private bool isGrounded;
    [SerializeField, ReadOnlyGUI] private bool isRunning;
    [SerializeField, ReadOnlyGUI] private bool canMove = true;
    [SerializeField, ReadOnlyGUI] private Vector3 moveDirection = Vector3.zero;

    private PlayerActions playerActions;
    private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction runAction;

    [Header("Reference")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Rigidbody playerRb;

    private void Awake()
    {
        playerActions = new();
        playerActions.PlayerCharacter.Enable();
        movementAction = playerActions.PlayerCharacter.Movement;
        jumpAction = playerActions.PlayerCharacter.Jump;
        runAction = playerActions.PlayerCharacter.Run;

    }

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

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            playerMovementConfig.groundDistance,
            playerMovementConfig.groundMask);

        Vector2 movementInput = movementAction.ReadValue<Vector2>();
        if (!isGrounded)
        {
            playerRb.drag = 0;
        }
        else
        {
            playerRb.drag = playerMovementConfig.groundDrag;
        }

        moveDirection = ((transform.forward * movementInput.y) + (transform.right * movementInput.x)).normalized;
        LimitVelocity();
    }
    private void FixedUpdate()
    {
        // Move player
        playerRb.AddForce(10 * playerMovementConfig.MoveSpeed * moveDirection, ForceMode.Force);

        if (!isGrounded)
        {
            playerRb.AddForce(-Physics.gravity.y * playerMovementConfig.gravityMultiplier * Vector3.down, ForceMode.Force);

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

    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            playerRb.velocity = new(playerRb.velocity.x, 0, playerRb.velocity.z);
            playerRb.AddForce(transform.up * playerMovementConfig.jumpPower, ForceMode.Impulse);

        }
    }

    private void PlayerRun(InputAction.CallbackContext context)
    {
        Debug.Log("Run");

        if (!isRunning && Input.GetKey(KeyCode.W))
        {
            isRunning = true;
        }
    }
    private void PlayerStopRun(InputAction.CallbackContext context)
    {
        Debug.Log("Stop run");
        if (isRunning && !Input.GetKey(KeyCode.W))
        {
            isRunning = false;
        }
    }

    private void OnEnable()
    {
        jumpAction.performed += PlayerJump;
        runAction.performed += PlayerRun;
        movementAction.canceled += PlayerStopRun;
    }

    private void OnDisable()
    {
        jumpAction.performed -= PlayerJump;
        runAction.performed -= PlayerRun;
        movementAction.canceled -= PlayerStopRun;

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
