using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public sealed class Explosion : WeaponBase
    {
        [SerializeField] private LayerSetting _enemyLayerSettings;
        [SerializeField] private VisualEffectHandler _explotionEffect;
        [SerializeField] private float _defaultRadius = 1f;
        
        private ExplosionDamage _explosionDamage;
        private ExplosionRadius _explosionRadius;

        protected override void OnInitialized()
        {
            _explosionRadius = OwnerEntity.StatContainer.Get<ExplosionRadius>();
            _explosionDamage = OwnerEntity.StatContainer.Get<ExplosionDamage>();
        }

        public async UniTask Explode()
        {
            UpdateExplotionRadius(_explosionRadius.Value);
            
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _explosionRadius.Value * _defaultRadius + 0.5f, _enemyLayerSettings.GetLayerMask());

            for (int i = 0; i < hitEnemies.Length; i++)
            {
                DamageEntity(_explosionDamage.Value, hitEnemies[i].GetComponent<CombatEntity>());
            }

            await _explotionEffect.PlayBurstEffect();
        }

        private void UpdateExplotionRadius(float explotionRaduis)
        {
            float scale = explotionRaduis;
            
            _explotionEffect.transform.localScale = new Vector3(scale, scale, scale);
        }
        
#if UNITY_EDITOR        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, _defaultRadius);
        }
#endif
    }
}