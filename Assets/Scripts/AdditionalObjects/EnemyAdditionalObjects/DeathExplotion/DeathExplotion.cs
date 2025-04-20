using UnityEngine;
using Cashing;
using Combat;

public sealed class DeathExplotion : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private Explotion _explotion;
    [SerializeField] private float _explotionRadius;
    [SerializeField] private float _explotionDamage;
    
    private void Start()
    {
        transform.SetParent(null);
        
        _ownerEntity.Health.Died += Explode;
        
        ExplotionDamage explotionDamage = new ExplotionDamage();
        ExplotionRadius explotionRadius = new ExplotionRadius();
        
        explotionDamage.SetDefault(_explotionDamage);
        explotionRadius.SetDefault(_explotionRadius);
        
        _ownerEntity.StatContainer.AddStat(explotionDamage);
        _ownerEntity.StatContainer.AddStat(explotionRadius);
        
        _explotion.Initialize(_ownerEntity);
    }

    private async void Explode()
    {
        transform.position = _ownerEntity.transform.position;
        _explotion.transform.SetParent(null);
        
        _ownerEntity.Health.Died -= Explode;
        
        _ownerEntity.StatContainer.Remove<ExplotionDamage>();
        _ownerEntity.StatContainer.Remove<ExplotionRadius>();
        
        await _explotion.Explode();
    }
}