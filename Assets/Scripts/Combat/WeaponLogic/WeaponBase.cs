using System;
using Combat;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    protected CombatEntity OwnerEntity;
    
    public Action<CombatEntity> HitEntity;
    public Action<CombatEntity> KilledEntity;
    
    public CombatEntity GetOwnerEntity() => OwnerEntity;
    
    public void Initialize(CombatEntity ownerEntity)
    {
        OwnerEntity = ownerEntity;

        OnInitialized();
    }

    public void DamageEntity(float damageAmount, CombatEntity receivingEntity)
    {
        if (!receivingEntity.Health.IsAlive()) return;
            
        float multipliedAttackDamage = OwnerEntity.DamageModifierContainer.DealerContainer.Modify(damageAmount, receivingEntity);
            
        receivingEntity.Health.ReceiveEnemyDamage(multipliedAttackDamage, OwnerEntity);
            
        HitEntity?.Invoke(receivingEntity);
            
        if (!receivingEntity.Health.IsAlive()) KilledEntity?.Invoke(receivingEntity);
    }
    
    protected virtual void OnInitialized() {}
}