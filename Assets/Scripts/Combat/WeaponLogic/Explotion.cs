using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Explotion : Weapon<CombatEntity>
    {
        [SerializeField] private LayerSetting _enemyLayerSettings;
    
        [SerializeField] private float _explotionRadius;
        public void SetExplotionRaduis(float value) => _explotionRadius = value;
    
        [SerializeField] private float _explotionDamage;
        public void SetExplotionDamage(float value) => _explotionDamage = value;
    
        [SerializeField] private VisualEffectHandler _explotionEffect;
    
        public void Explode()
        {
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _explotionRadius, _enemyLayerSettings.GetLayerMask());
    
            foreach (var t in hitEnemies)
            {
                DamageEntity(t.transform.gameObject.GetComponent<CombatEntity>(), _explotionDamage);
            }
            
            Instantiate(_explotionEffect, transform.position, Quaternion.identity).Play();
    
            Destroy(gameObject);
        }
    
        public void AwaitExplode(float time)
        {
            Invoke("Explode", time);
        }
    }
}