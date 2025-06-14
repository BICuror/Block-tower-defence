using System;
using Combat;

public abstract class EntityEffect
{
    protected ArgumentsContainer ArgumentsContainer;
    protected CombatEntity Entity;
    public int MaxStacks;
    public int TrueStack;
    public int Stack;
    
    public abstract EntityEffectType EffectType { get; }
    public virtual bool CanBeApplied() => true;
    
    public void Initialize(ArgumentsContainer argumentsContainer, int maxStacks)
    {
        ArgumentsContainer = argumentsContainer;
        MaxStacks = maxStacks;
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