using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

namespace Combat
{
    public sealed class MortarProjectile : PlayerWeapon
    {
        [SerializeField] Explotion _explotion; 
        [SerializeField] private LayerSetting _enemyLayerSettings; 
        [SerializeField] private AnimationCurve _heightCurve; 
        [SerializeField] private float _maxHeight;
        
        private Damage _damage;

        protected override void OnInitialized()
        {
            _damage = OwnerEntity.StatContainer.Get<Damage>();
            _explotion.Initialize(OwnerEntity);
        }

        public async UniTask TravelToPoint(Vector3 finalPosition, float travelTime)
        {   
            Vector3 startPosition = transform.position;
    
            float time = 0f;
    
            while (time < travelTime)
            {
                float evaluatedTime = time / travelTime;
    
                Vector3 currentPosition = Vector3.Lerp(startPosition, finalPosition, evaluatedTime);
    
                currentPosition.y += _heightCurve.Evaluate(evaluatedTime) * _maxHeight;
    
                transform.position = currentPosition;
    
                time += Time.fixedDeltaTime;

                try
                {
                    await UniTask.WaitForFixedUpdate(cancellationToken: destroyCancellationToken);
                }
                catch (Exception e) { TaskUtility.LogAsync(e); }
            }

            Collider.enabled = false;
            await _explotion.Explode();
            Disable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CombatEntity enemyEntity))
            {
                DamageEntity(_damage.Value, enemyEntity);
            }
        }
    }
}