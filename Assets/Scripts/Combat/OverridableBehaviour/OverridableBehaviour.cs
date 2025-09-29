using System.Collections.Generic;
using System;
using Combat;

public abstract class OverridableBehaviourBase<T> where T : CombatBehaviourCore
{
    private CombatEntity _ownerEntity;
    protected bool IsOverriden;
    protected T DefaultBehaviour;
    protected T OverrideBehaviour;
    protected List<T> AdditionalBehaviours = new();

    public void Initialize(CombatEntity ownerEntity, T defaultBehaviour = null)
    {
        _ownerEntity = ownerEntity;
        DefaultBehaviour = defaultBehaviour;
        
        if (DefaultBehaviour != null) DefaultBehaviour.SetOwnerEntity(_ownerEntity);
    }
    
    public void AddBehaviour(T behaviour, BehaviourType behaviourType)
    {
        behaviour.SetOwnerEntity(_ownerEntity);
        
        if (behaviourType == BehaviourType.Additional)
        {
            AdditionalBehaviours.Add(behaviour);
        }
        else
        {
            if (!IsOverriden)
            {
                OverrideBehaviour = behaviour;
                IsOverriden = true;
            }
            else throw new Exception("Override behaviour has already been added.");
        }
    }

    public void RemoveOverrideBehaviour()
    {
        OverrideBehaviour = null; 
        IsOverriden = false;
    }

    public void RemoveAdditionalBehaviour(T behaviour)
    {
        AdditionalBehaviours.Remove(behaviour);
    }
}

public sealed class OverridableBehaviour : OverridableBehaviourBase<CombatBehaviour>
{
    public void Execute()
    {
        if (IsOverriden) OverrideBehaviour.Execute();
        else if (DefaultBehaviour != null) DefaultBehaviour.Execute();

        for (int i = 0; i < AdditionalBehaviours.Count; i++)
        {
            AdditionalBehaviours[i].Execute();
        }
    }
}

public sealed class OverridableBehaviour<T> : OverridableBehaviourBase<CombatBehaviour<T>>
{
    public void Execute(T arg)
    {
        if (IsOverriden) OverrideBehaviour.Execute(arg);
        else if (DefaultBehaviour != null) DefaultBehaviour.Execute(arg);

        for (int i = 0; i < AdditionalBehaviours.Count; i++)
        {
            AdditionalBehaviours[i].Execute(arg);
        }
    }
}

public enum BehaviourType
{
    Override,
    Additional
}