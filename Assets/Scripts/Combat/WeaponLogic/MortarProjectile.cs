using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public sealed class MortarProjectile : Weapon
    {
        [SerializeField] private LayerSetting _enemyLayerSettings; 
        [SerializeField] private AnimationCurve _heightCurve; 
        [SerializeField] private float _maxHeight;
        
        private Damage _damage;
        private TravelTime _travelTime;

        protected override void OnInitialized()
        {
            _damage = OwnerEntity.StatContainer.Get<Damage>();
            _travelTime = OwnerEntity.StatContainer.Get<TravelTime>();
        }

        public async UniTask TravelToPoint(Vector3 finalPosition)
        {
            Vector3 startPosition = transform.position;
    
            float time = 0f;
    
            while (time < _travelTime.Value)
            {
                float evaluatedTime = time / _travelTime.Value;
    
                Vector3 currentPosition = Vector3.Lerp(startPosition, finalPosition, evaluatedTime);
    
                currentPosition.y += _heightCurve.Evaluate(evaluatedTime) * _maxHeight;
    
                transform.position = currentPosition;
    
                time += Time.fixedDeltaTime; 
                
                await UniTask.WaitForFixedUpdate();
            }

            Collider.enabled = false;
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