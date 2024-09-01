using UnityEngine;
using UnityEngine.VFX;

public sealed class Arrow : PlayerWeapon
{
    [SerializeField] private VisualEffectHandler _visualEffectHandler;

    private void Awake() 
    {
        HitSomething.AddListener(OnHitSomehing);
    }

    private void OnHitSomehing()
    {
        _visualEffectHandler.Play();

        Rigidbody.velocity = Vector3.zero;
    }
}
