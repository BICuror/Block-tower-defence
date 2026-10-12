using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;

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
            DamageEntity(_damage.Value, enemyEntity);
        }
    }
}