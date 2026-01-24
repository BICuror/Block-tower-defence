using UnityEngine;
using System;

namespace Combat
{
    public sealed class Arrow : Weapon
    {
        [SerializeField] private VisualEffectHandler _visualEffectHandler;
        private Damage _damage;

        public event Action<Arrow> OnArrowHit;
        
        protected override void OnInitialized()
        {
            _damage = OwnerEntity.StatContainer.Get<Damage>();
        }

        public void Launch(float speed, Vector3 targetPosition, Vector3 shootingPosition)
        {
            Rigidbody.linearVelocity = Vector3.zero;
            transform.position = shootingPosition;

            transform.LookAt(targetPosition);

            Rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CombatEntity enemyEntity))
            {
                DamageEntity(_damage.Value, enemyEntity);
                
                OnArrowHit?.Invoke(this);
            }
        }

        public void DisableArrow()
        {
            _visualEffectHandler.PlayBurstEffectAndForget();
            SetState(false);
        }
    }
}