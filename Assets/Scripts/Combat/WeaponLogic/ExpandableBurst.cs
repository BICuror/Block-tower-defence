using Cysharp.Threading.Tasks;
using Combat.Stats;
using UnityEngine;
using Combat;

public sealed class ExpandableBurst : Weapon
{
    private Damage _damage;
    
    public async UniTask EnableAndExpand()
    {
        
    }
    
    protected override void OnInitialized()
    {
        _damage = OwnerEntity.StatContainer.Get<Damage>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CombatEntity enemyEntity))
        {
            DamageEntity(_damage.GetValueModificationContainer(), enemyEntity);
        }
    }
}