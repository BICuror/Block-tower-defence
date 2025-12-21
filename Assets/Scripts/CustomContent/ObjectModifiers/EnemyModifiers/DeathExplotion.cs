using UnityEngine;
using Cashing;
using Combat;

public sealed class DeathExplotion : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private Explosion _explosion;
    
    private void Start()
    {
        transform.SetParent(null);

        _ownerEntity.ComponentsContainer.Get<EnemyContactDamager>().OnDealingContactDamage += KillEntity;
        _ownerEntity.Health.Died += Explode;
        
        _explosion.Initialize(_ownerEntity);
    }

    private void KillEntity() => _ownerEntity.Health.Die();

    private async void Explode()
    {
        transform.position = _ownerEntity.transform.position;
        
        _ownerEntity.ComponentsContainer.Get<EnemyContactDamager>().OnDealingContactDamage -= KillEntity;
        _ownerEntity.Health.Died -= Explode;
        
        _explosion.transform.SetParent(null);
        await _explosion.Explode();
    }
}