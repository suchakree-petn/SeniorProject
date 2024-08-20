using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class JigsawWorldObject : NetworkBehaviour, ICollectable
{
    public uint JigsawId;


    [ServerRpc(RequireOwnership = false)]
    private void Collect_ServerRpc()
    {
        Collect_ClientRpc();
    }

    [ClientRpc]
    private void Collect_ClientRpc()
    {
        Map2_PuzzleManager.Instance.Puzzle2_CollectJigsaw(JigsawId);
    }

    public void Collect()
    {
        Collect_ServerRpc();
    }
}
