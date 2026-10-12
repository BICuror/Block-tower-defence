using System;
using Combat;

public sealed class EntityValueModifierContainer
{
    private readonly ValueModifierContainer _damageReceiverContainer;
    private readonly ValueModifierContainer _damageDealerContainer;
    private readonly ValueModifierContainer _healReceiverContainer;
    private readonly ValueModifierContainer _healDealerContainer;
    
    public ValueModifierContainer DamageReceiverContainer => _damageReceiverContainer;
    public ValueModifierContainer DamageDealerContainer => _damageDealerContainer;
    public ValueModifierContainer HealReceiverContainer => _healReceiverContainer;
    public ValueModifierContainer HealDealerContainer => _healDealerContainer;

    //invoked before entity.Die()
    public event Action<CombatEntity> EntityKilled;
    //invoked before entity.Die()
    public event Action<CombatEntity> EntityDied;
    public event Action<float> DamageDealt;
    public event Action<CombatEntity> EnemyHit;
    
    public EntityValueModifierContainer(CombatEntity ownerEntity)
    {
        _damageReceiverContainer = new(ownerEntity);
        _damageDealerContainer = new(ownerEntity);
        _healReceiverContainer = new(ownerEntity);
        _healDealerContainer = new(ownerEntity);
    }
    
    public void InvokeOnEnemyHit(CombatEntity hitEntity)
    {
        EnemyHit?.Invoke(hitEntity);
    }
    
    public void InvokeOnDamageDealt(float damage)
    {
        DamageDealt?.Invoke(damage);
    }

    public void InvokeOnKillEffects(CombatEntity killedEntity)
    {
        EntityKilled?.Invoke(killedEntity);
    }

    public void InvokeOnDeathEffects(CombatEntity killerEntity)
    {
        EntityDied?.Invoke(killerEntity);
    }
}