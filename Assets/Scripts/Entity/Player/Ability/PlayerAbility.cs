using Unity.Netcode;
using UnityEngine.UI;

public abstract class PlayerAbility: NetworkBehaviour
{

    public abstract void ActivateAbility(ulong userClientId);
}
