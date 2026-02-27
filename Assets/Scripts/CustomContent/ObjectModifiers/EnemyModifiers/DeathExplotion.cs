using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class DeathExplotion : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private Explosion _explosion;
    
    private void Start()
    {
        _ownerEntity.ComponentsContainer.Get<EnemyContactDamager>().OnDealingContactDamage += KillEntity;
        _ownerEntity.Health.Died += Explode;
        
        _explosion.Initialize(_ownerEntity);
    }

    private void KillEntity() => _ownerEntity.Health.Die();

    private void Explode()
    {
        _ownerEntity.ComponentsContainer.Get<EnemyContactDamager>().OnDealingContactDamage -= KillEntity;
        _ownerEntity.Health.Died -= Explode;
        
        _explosion.Explode().Forget();
    }
}