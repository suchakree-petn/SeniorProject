using UnityEngine;

public class DefaultArrow : Arrow
{
    public override void OnCollisionEnter(Collision other)
    {
        Debug.Log($"Arrow {name} collide with {other.gameObject.name}");
        SetKinematic();
    }
}
