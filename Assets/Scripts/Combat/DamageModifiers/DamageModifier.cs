using Combat;

public abstract class DamageModifier
{
    protected CombatEntity OwnerEntity;
    protected ArgumentsContainer Args;
    public virtual ResolveOrder Order => ResolveOrder.Default;

    public void SetOwner(CombatEntity ownerEntity) => OwnerEntity = ownerEntity;
    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    
    public abstract float Modify(CombatEntity otherEntity, float value);
}

public enum ResolveOrder
{
    Default,
    Final,
    Single
}