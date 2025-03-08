using UnityEngine;

namespace Combat
{
    public sealed class Arrow : PlayerWeapon
    {
        [SerializeField] private VisualEffectHandler _visualEffectHandler;
        private Damage _damage;
        
        protected override void OnInitialized()
        {
            _damage = OwnerEntity.StatContainer.Get<Damage>();
        }

        private async void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EnemyEntity enemyEntity))
            {
                DamageEntity(_damage.Value, enemyEntity);

                Collider.enabled = false;
                
                Rigidbody.velocity = Vector3.zero;
                await _visualEffectHandler.Play();
                
                Disable();
            }
        }
    }
}