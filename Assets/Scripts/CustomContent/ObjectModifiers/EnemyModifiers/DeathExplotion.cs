using UnityEngine;
using Cashing;
using Combat;
using UnityEngine.Serialization;

public sealed class DeathExplotion : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [FormerlySerializedAs("_explotion")] [SerializeField] private Explosion _explosion;
    [SerializeField] private float _explotionRadius;
    [SerializeField] private float _explotionDamage;
    
    private void Start()
    {
        transform.SetParent(null);
        
        _ownerEntity.Health.Died += Explode;
        
        ExplosionDamage explosionDamage = new ExplosionDamage();
        ExplosionRadius explosionRadius = new ExplosionRadius();
        
        explosionDamage.SetDefault(_explotionDamage);
        explosionRadius.SetDefault(_explotionRadius);
        
        _ownerEntity.StatContainer.AddStat(explosionDamage);
        _ownerEntity.StatContainer.AddStat(explosionRadius);
        
        _explosion.Initialize(_ownerEntity);
    }

    private async void Explode()
    {
        transform.position = _ownerEntity.transform.position;
        _explosion.transform.SetParent(null);
        
        _ownerEntity.Health.Died -= Explode;
        
        _ownerEntity.StatContainer.Remove<ExplosionDamage>();
        _ownerEntity.StatContainer.Remove<ExplosionRadius>();
        
        await _explosion.Explode();
    }
}