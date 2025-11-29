using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace Combat
{
    public sealed class Arrow : Weapon
    {
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private VisualEffectHandler _visualEffectHandler;
        private Damage _damage;

        public Action<Arrow> OnArrowHit;
        
        protected override void OnInitialized()
        {
            _damage = OwnerEntity.StatContainer.Get<Damage>();
        }

        public void Launch(float speed, Vector3 targetPosition, Vector3 shootingPosition)
        {
            Rigidbody.velocity = Vector3.zero;
            transform.position = shootingPosition;
            _trailRenderer.Clear();

            transform.LookAt(targetPosition);

            Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EnemyEntity enemyEntity))
            {
                DamageEntity(_damage.Value, enemyEntity);
                
                OnArrowHit?.Invoke(this);
            }
        }

        public async UniTask DisableArrow()
        {
            Collider.enabled = false;
            Rigidbody.velocity = Vector3.zero;
            
            await _visualEffectHandler.PlayAndStop();
            
            Disable();
        }
    }
}