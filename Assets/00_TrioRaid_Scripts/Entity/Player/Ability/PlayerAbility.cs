using Unity.Netcode;
using UnityEngine;

public abstract class PlayerAbility : NetworkBehaviour
{
    public bool CanUse = true;
    public bool IsActive = false;
    protected bool IsOnCD;
    public KeyCode ActivateKey;
    public abstract void ActivateAbility(ulong userClientId);

    protected virtual void Update()
    {
        if (!IsOwner) return;

        if (!CanUse) return;

        if (Input.GetKeyDown(ActivateKey) && !IsOnCD && !GetComponent<PlayerController>().IsDead)
        {
            Debug.Log("Use Ability");
            ActivateAbility(OwnerClientId);
        }
    }

    public void SetOnCD()
    {
        IsOnCD = true;
    }

    public void SetFinishCD()
    {
        IsOnCD = false;
    }
}
