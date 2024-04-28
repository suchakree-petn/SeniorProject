using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : NetworkSingleton<AbilityManager>
{
    public PlayerAbility playerAbility;
    protected override void InitAfterAwake()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerAbility.ActivateAbility(OwnerClientId);
        }
    }

}
