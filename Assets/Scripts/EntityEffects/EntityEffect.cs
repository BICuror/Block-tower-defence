using System;
using Combat;
using Random = UnityEngine.Random;

public abstract class EntityEffect
{
    private EntityEffectData _effectData;
    protected ArgumentsContainer ArgumentsContainer;
    protected CombatEntity Entity;
    public int TrueStack;
    public int Stack;
    
    public int MaxStacks => _effectData.MaxStacks;
    public EntityEffectData EffectData => _effectData;
    
    public abstract EntityEffectType EffectType { get; }
    public virtual bool CanBeApplied() => true;
    
    public void Initialize(ArgumentsContainer argumentsContainer, EntityEffectData effectData)
    {
        ArgumentsContainer = argumentsContainer;
        _effectData = effectData;
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