using Unity.Netcode;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class VineTrap : NetworkBehaviour
{
    public float StopMoveDuration = 5;
    public float PopOffset;
    public float PopDuration;
    private bool isFinishDrop = false;
    private bool isTriggered = false;
    private EnemyController target;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        isFinishDrop = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !IsSpawned || !isFinishDrop || isTriggered) return;
        if (other.transform.root.TryGetComponent(out target))
        {
            isTriggered = true;
            target.StopMoving();
            GetComponent<Rigidbody>().useGravity = false;
            gameObject.isStatic = true;
            transform.DOMoveY(transform.position.y + PopOffset, PopDuration);

            Invoke(nameof(DestroyTrap), StopMoveDuration);
        }
    }

    private void DestroyTrap()
    {
        if (target != null)
        {
            target.Moving();
        }

        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
