using UnityEngine;

namespace Combat
{
    public sealed class EnemyArrow : EnemyWeapon
    {
        [SerializeField] private VisualEffectHandler _visualEffectHandler;
    
        private void Awake() => HitEntity += _ => OnHitSomehing();
    
        private void OnHitSomehing()
        {
            _visualEffectHandler.Play();
    
            Rigidbody.velocity = Vector3.zero;
        }
    
        private void OnBecameInvisible() => gameObject.SetActive(false);
    }
}