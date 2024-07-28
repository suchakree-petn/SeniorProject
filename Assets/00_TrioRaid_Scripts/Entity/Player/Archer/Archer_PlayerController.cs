using UnityEngine;

public partial class Archer_PlayerController : PlayerController
{
    [Header("Archer Reference")]
    [SerializeField] protected Archer_PlayerWeapon archer_playerWeapon;
    [SerializeField] protected ArcherAbility_ArrowManiac archerAbility_ArrowManiac;
    [SerializeField] protected ArcherAbility_VineTrap archerAbility_VineTrap;


    protected override void Start()
    {
        if (!IsOwner) return;

        base.Start();
        AbilityUIManager.Instance.OnSetAbilityIcon_E?.Invoke(archerAbility_ArrowManiac.AbilityData.Icon);
        AbilityUIManager.Instance.OnSetAbilityIcon_Q?.Invoke(archerAbility_VineTrap.archerAbilityData.Icon);

    }

    protected override void Update()
    {
        if (!IsOwner) return;
        base.Update();
        // ArcherAnimation();
    }
    protected override void LateUpdate()
    {
        if (!IsOwner) return;
        base.LateUpdate();
        ArcherAnimation();
    }

    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        base.OnNetworkSpawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += archer_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled += archer_playerWeapon.UseWeapon;

    }

    public override void OnNetworkDespawn()
    {

        if (!IsOwner) return;
        base.OnNetworkDespawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= archer_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled -= archer_playerWeapon.UseWeapon;

    }


    private void ArcherAnimation()
    {
        if (PlayerCameraMode.Focus == PlayerCameraMode)
        {
            PlayerAnimation.SetLayerWeight(1, Mathf.Lerp(PlayerAnimation.GetLayerWeight(1), 1, Time.deltaTime * 10));
            WalkAnimationWhileFocus();

            if (archer_playerWeapon.IsDrawing)
            {
                PlayerAnimation.SetLayerWeight(2, 1);
                DrawingBowAnimation();
            }
            else
            {
                PlayerAnimation.SetLayerWeight(2, 0);
                PlayerAnimation.SetFloat("DrawPower", 0);
            }
        }
        else
        {
            PlayerAnimation.SetLayerWeight(2, 0);
            PlayerAnimation.SetLayerWeight(1, Mathf.Lerp(PlayerAnimation.GetLayerWeight(1), 0, Time.deltaTime * 10));
        }
    }

    private void WalkAnimationWhileFocus()
    {
        Vector3 finalVelocity = playerMovement.GetMovementForce();
        finalVelocity = new(finalVelocity.x, 0f, finalVelocity.z);
        finalVelocity = Vector3.ClampMagnitude(finalVelocity, playerMovement.PlayerMovementConfig.MoveSpeed);
        finalVelocity = transform.InverseTransformDirection(finalVelocity); // Convert to local space
        finalVelocity = playerMovement.GetMoveSpeedRatioOfNormalMoveSpeed(finalVelocity);
        if (PlayerInputManager.Instance.MovementAction.IsPressed())
        {
            PlayerAnimation.SetMoveVelocityX(finalVelocity.x);
            PlayerAnimation.SetMoveVelocityZ(finalVelocity.z);
        }
        else
        {
            finalVelocity -= playerMovement.PlayerMovementConfig.groundDrag * Time.fixedDeltaTime * finalVelocity;
            PlayerAnimation.SetMoveVelocityX(finalVelocity.x);
            PlayerAnimation.SetMoveVelocityZ(finalVelocity.z);
        }
    }
    private void DrawingBowAnimation()
    {
        float drawPowerRatio = archer_playerWeapon.DrawPower / archer_playerWeapon.BowConfig.MaxDrawPower;
        PlayerAnimation.SetFloat("DrawPower", drawPowerRatio);
    }

    public Archer_PlayerWeapon GetArcherWeapon()
    {
        return archer_playerWeapon;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!IsOwner) return;

    }

    protected override void OnDisable()
    {

        base.OnDisable();
        if (!IsOwner) return;
    }
}
