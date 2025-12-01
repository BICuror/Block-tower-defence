using Combat;

public abstract class CombatBehaviourCore
{
    protected ArgumentsContainer Args;
    protected CombatEntity Entity;
    
    public void SetArgumentsContainer(ArgumentsContainer args)
    {
        Args = args;
    }
    
    public void SetOwnerEntity(CombatEntity ownerEntity)
    {
        Entity = ownerEntity;

        OnOwnerEntitySet();
    }

    protected virtual void OnOwnerEntitySet() {}
}

public abstract class CombatBehaviour : CombatBehaviourCore
{
    public abstract void Execute();
}

public abstract class CombatBehaviour<T> : CombatBehaviourCore
{
    public abstract void Execute(T dynamicArg);
}