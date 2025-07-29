using System;
using Combat;

public sealed class EntityDamageModifierContainer
{
    private DamageModifierContainer _reciverContainer;
    private DamageModifierContainer _dealerContainer;
    
    public DamageModifierContainer ReciverContainer => _reciverContainer;
    public DamageModifierContainer DealerContainer => _dealerContainer;

    public Action<CombatEntity> EntityKilled;
    public Action<CombatEntity> EntityDied;
    
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