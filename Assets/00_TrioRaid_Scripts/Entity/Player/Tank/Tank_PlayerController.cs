using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Tank_PlayerController : PlayerController
{
    [Header("Tank Reference")]
    public GameObject LockEnemyTarget;
    [SerializeField] private float moveWhileComboMaxDistance;
    [SerializeField] private float moveWhileComboDuration;
    [SerializeField] protected Tank_PlayerWeapon tank_playerWeapon;
    [SerializeField] protected TankAbility_BarbarianShout tankAbility_BarbarianShout;
    [SerializeField] protected TankAbility_GroundSmash tankAbility_GroundSmash;

    protected override void Start()
    {
        base.Start();
        AbilityUIManager.Instance.OnSetAbilityIcon_E?.Invoke(tankAbility_GroundSmash.AbilityData.Icon);
        AbilityUIManager.Instance.OnSetAbilityIcon_Q?.Invoke(tankAbility_BarbarianShout.AbilityData.Icon);
    }

    protected override void Update()
    {
        if (!IsOwner) return;
        base.Update();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchLockTarget();
        }

        if (tank_playerWeapon.ComboTimeInterval > 0)
        {
            PlayerInputManager.Instance.MovementAction.Disable();
        }
        else
        {
            PlayerInputManager.Instance.MovementAction.Enable();
        }
    }

    protected override void FixedUpdate()
    {
        if (!IsOwner) return;
        base.FixedUpdate();

    }



    protected override void LateUpdate()
    {
        if (!IsOwner) return;
        base.LateUpdate();

        PlayerUIManager playerUIManager = PlayerUIManager.Instance;
        if (LockEnemyTarget != null)
        {
            if (!LockEnemyTarget.GetComponent<EnemyController>().IsDead)
            {
                playerUIManager.SetLockTargetState(true);
                playerUIManager.SetLockTargetPosition(LockEnemyTarget.transform.position, false);
            }
            else
            {
                playerUIManager.SetLockTargetState(false);
            }
        }
        else
        {
            playerUIManager.SetLockTargetState(false);
        }
        // TankAnimation();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        base.OnNetworkSpawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += LockTarget;
        playerInputManager.Attack.performed += tank_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled += tank_playerWeapon.UseWeapon;

    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        base.OnNetworkDespawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= LockTarget;
        playerInputManager.Attack.performed -= tank_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled -= tank_playerWeapon.UseWeapon;

    }
    private void MoveToLockTargetWhenAttack()
    {
        if (LockEnemyTarget == null || LockEnemyTarget.GetComponent<EnemyController>().IsDead)
        {
            playerMovement.PlayerRigidbody.AddForce(transform.forward * moveWhileComboMaxDistance, ForceMode.Impulse);
            // playerMovement.PlayerRigidbody.DOMove(transform.position + (transform.forward * moveWhileComboMaxDistance), moveWhileComboDuration).SetEase(Ease.InSine);
        }
        else
        {
            float distance = Vector3.Distance(transform.position, LockEnemyTarget.transform.position);
            if (distance < tank_playerWeapon.SwordWeaponData.NA_AttackRange)
            {
                Debug.Log("No move");
                return;
            }
            Vector3 startPos = transform.position;
            playerMovement.PlayerRigidbody.AddForce(LockEnemyTarget.transform.position - startPos, ForceMode.Impulse);

            // playerMovement.PlayerRigidbody.DOMove(LockEnemyTarget.transform.position, moveWhileComboDuration).SetEase(Ease.InSine).OnUpdate(() =>
            // {
            //     Vector3 currentPos = transform.position;

            //     float distance = Vector3.Distance(transform.position, LockEnemyTarget.transform.position);
            //     if (Vector3.Distance(currentPos, startPos) > moveWhileComboMaxDistance || distance < tank_playerWeapon.SwordWeaponData.NA_AttackRange)
            //     {
            //         playerMovement.PlayerRigidbody.DOKill();
            //     }
            // });
        }
    }
    private void LockTarget(InputAction.CallbackContext context)
    {
        if (LockEnemyTarget == null || LockEnemyTarget.GetComponent<EnemyController>().IsDead)
        {
            LockEnemyTarget = GetClosestEnemy();
        }
        else
        {
            Vector3 direction = (LockEnemyTarget.transform.position - transform.position).normalized;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            transform.forward = direction;
        }

    }

    public GameObject GetClosestEnemy()
    {
        GameObject[] allEnemyInScene = GameObject.FindGameObjectsWithTag("Enemy");

        if (allEnemyInScene.Length == 0) return null;

        float closestEnemyDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < allEnemyInScene.Length; i++)
        {
            if (allEnemyInScene[i].GetComponent<EnemyController>().IsDead) continue;

            Transform enemyTransform = allEnemyInScene[i].transform;
            float distance = Vector3.Distance(enemyTransform.position, transform.root.position);
            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                index = i;
            }
        }
        return allEnemyInScene[index];
    }

    private void SwitchLockTarget()
    {
        if (LockEnemyTarget != null)
        {
            if (LockEnemyTarget.GetComponent<EnemyController>().IsDead)
            {
                LockTarget(new());
                return;
            }
        }
        else
        {
            LockTarget(new());
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit[] hits = Physics.SphereCastAll(ray, 8, 50, PlayerCharacterData.TargetLayer);
        List<GameObject> allEnemy = new();
        foreach (RaycastHit hit in hits)
        {
            GameObject enemy = hit.transform.root.gameObject;
            if (!allEnemy.Contains(enemy) && !enemy.GetComponent<EnemyController>().IsDead)
            {
                Debug.Log("Hits " + hit.transform.gameObject.name);
                allEnemy.Add(enemy);
            }

        }

        int lockIndex = 0;
        if (allEnemy.Count > 1)
        {
            foreach (GameObject enemy in allEnemy)
            {
                if (enemy == LockEnemyTarget)
                {
                    lockIndex = allEnemy.IndexOf(enemy);
                    break;
                }
            }
        }
        else
        {
            return;
        }

        lockIndex++;
        if (lockIndex > allEnemy.Count - 1)
        {
            lockIndex = 0;
        }

        LockEnemyTarget = allEnemy[lockIndex];

    }

    public void SetWeaponHolderPosition(PlayerCameraMode prev, PlayerCameraMode current)
    {
        if (current == PlayerCameraMode.Focus)
        {
            // tank_playerWeapon.SwitchWeaponHolderTo(Tank_WeaponHolderState.InHand);
            tank_playerWeapon.WeaponHolderState = Tank_WeaponHolderState.InHand;
        }
        else
        {
            // tank_playerWeapon.SwitchWeaponHolderTo(Tank_WeaponHolderState.OnBack);
            tank_playerWeapon.WeaponHolderState = Tank_WeaponHolderState.OnBack;
        }

    }

    // private void TankAnimation()
    // {

    //     if (tank_playerWeapon.WeaponHolderState == Tank_WeaponHolderState.InHand)
    //     {
    //         WalkAnimationWhileFocus();

    //         if (tank_playerWeapon.IsSlash)
    //         {
    //             playerAnimation.SetBool("IsHSlash", true);
    //         }
    //         else
    //         {
    //             playerAnimation.SetBool("IsHSlash", false);
    //         }
    //     }
    //     else
    //     {
    //         if (tank_playerWeapon.IsSlash)
    //         {
    //             playerAnimation.SetBool("IsLSlash", true);
    //         }
    //         else
    //         {
    //             playerAnimation.SetBool("IsLSlash", false);
    //         }
    //     }
    // }

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
    public Tank_PlayerWeapon GetTank_PlayerWeapon()
    {
        return tank_playerWeapon;
    }

    public override void SwitchViewMode(InputAction.CallbackContext context)
    {
        // No focus mode for tank;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        OnPlayerCameraModeChanged += SetWeaponHolderPosition;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnPlayerCameraModeChanged -= SetWeaponHolderPosition;
    }
}
