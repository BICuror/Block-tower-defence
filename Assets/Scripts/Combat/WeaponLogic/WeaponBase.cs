using System;
using Combat;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    protected CombatEntity OwnerEntity;
    
    public event Action<CombatEntity> HitEntity;
    public event Action<CombatEntity> KilledEntity;
    
    public CombatEntity GetOwnerEntity() => OwnerEntity;
    
    public void Initialize(CombatEntity ownerEntity)
    {
        OwnerEntity = ownerEntity;

        OnInitialized();
    }

    public void DamageEntity(float damageAmount, CombatEntity receivingEntity)
    {
        if (!receivingEntity.Health.IsAlive()) return;
            
        receivingEntity.Health.ReceiveEnemyDamage(damageAmount, OwnerEntity);
            
        HitEntity?.Invoke(receivingEntity);
            
        if (!receivingEntity.Health.IsAlive()) KilledEntity?.Invoke(receivingEntity);
    }
    
    protected virtual void OnInitialized() {}
    
    protected virtual void SetState(bool state)
    {
        gameObject.SetActive(state);
    }
}