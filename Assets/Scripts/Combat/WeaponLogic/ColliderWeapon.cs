using UnityEngine;

public class ColliderWeapon : WeaponBase
{
    [SerializeField] protected Rigidbody Rigidbody;
    [SerializeField] protected Collider Collider;

    protected override void SetState(bool state)
    {
        base.SetState(state);
        
        Collider.enabled = state;
    }
}