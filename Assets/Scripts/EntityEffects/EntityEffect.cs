using System;
using Combat;
using Random = UnityEngine.Random;

public abstract class EntityEffect
{
    private EntityEffectConfig _effectConfig;
    protected ArgumentsContainer ArgumentsContainer;
    protected CombatEntity Entity;
    public int TrueStack;
    public int Stack;
    
    public int MaxStacks => _effectConfig.MaxStacks;
    public EntityEffectConfig EffectConfig => _effectConfig;
    
    public abstract EntityEffectType EffectType { get; }
    public virtual bool CanBeApplied() => true;
    
    public void Initialize(ArgumentsContainer argumentsContainer, EntityEffectConfig effectConfig)
    {
        ArgumentsContainer = argumentsContainer;
        _effectConfig = effectConfig;
        OnInitialized();
    }

    protected virtual void OnInitialized() {}
    
    public void SetEntity(CombatEntity entity) => Entity = entity;

    public void SetStack(int strength)
    {
        TrueStack = strength;
        Stack = Math.Clamp(TrueStack, 0, MaxStacks);
    }
    
    public void ChangeStack(int strengthIncrease)
    {
        TrueStack += strengthIncrease;
        Stack = Math.Clamp(TrueStack, 0, MaxStacks);
    }
    
    public virtual void Update() {}
    public abstract void ApplyToEntity();
    public abstract void RemoveFromEntity();
}

public enum EntityEffectType
{
    Positive,
    Negative
}