using UnityEngine;

namespace Combat
{
    public sealed class Arrow : Weapon
    {
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private VisualEffectHandler _visualEffectHandler;
        private Damage _damage;
        private bool _isPiercing;
        
        protected override void OnInitialized()
        {
            _damage = OwnerEntity.StatContainer.Get<Damage>();
        }

        private async void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EnemyEntity enemyEntity))
            {
                DamageEntity(_damage.Value, enemyEntity);

                if (_isPiercing) return;
                
                Collider.enabled = false;
                Rigidbody.velocity = Vector3.zero;
                await _visualEffectHandler.Play();
                Disable();
                _trailRenderer.Clear();
            }
        }

        public void SetPiercingState(bool state) => _isPiercing = state;
    }
}