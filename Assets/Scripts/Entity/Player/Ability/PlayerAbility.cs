using Unity.Netcode;
using UnityEngine;

public abstract class PlayerAbility : NetworkBehaviour
{
    protected bool IsOnCD;
    public KeyCode ActivateKey;
    public abstract void ActivateAbility(ulong userClientId);

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(ActivateKey) && !IsOnCD && !GetComponent<PlayerController>().IsPlayerDie)
        {
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
