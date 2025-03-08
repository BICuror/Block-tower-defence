using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public sealed class Explotion : Weapon
    {
        [SerializeField] private LayerSetting _enemyLayerSettings;
        [SerializeField] private VisualEffectHandler _explotionEffect;
        [SerializeField] private float _defaultRadius = 1f;
        
        private ExplotionDamage _explotionDamage;
        private ExplotionRadius _explotionRadius;
        
        public async UniTask Explode()
        {
            _explotionRadius = OwnerEntity.StatContainer.Get<ExplotionRadius>();
            _explotionDamage = OwnerEntity.StatContainer.Get<ExplotionDamage>();

            UpdateExplotionRadius(_explotionRadius.Value);
            
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _explotionRadius.Value, _enemyLayerSettings.GetLayerMask());

            for (int i = 0; i < hitEnemies.Length; i++)
            {
                DamageEntity(_explotionDamage.Value, hitEnemies[i].GetComponent<CombatEntity>());
            }

            await _explotionEffect.Play();
        }

        private void UpdateExplotionRadius(float explotionRaduis)
        {
            float scale = _defaultRadius * explotionRaduis;
            
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