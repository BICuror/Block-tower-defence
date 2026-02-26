using System;
using Combat;

public sealed class EntityDamageModifierContainer
{
    private DamageModifierContainer _reciverContainer;
    private DamageModifierContainer _dealerContainer;
    
    public DamageModifierContainer ReciverContainer => _reciverContainer;
    public DamageModifierContainer DealerContainer => _dealerContainer;

    //invoked before entity.Die()
    public event Action<CombatEntity> EntityKilled;
    //invoked before entity.Die()
    public event Action<CombatEntity> EntityDied;
    
    public EntityDamageModifierContainer(CombatEntity ownerEntity)
    {
        _reciverContainer = new(ownerEntity);
        _dealerContainer = new(ownerEntity);
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