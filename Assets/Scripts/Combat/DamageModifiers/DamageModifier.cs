using Combat;

public abstract class DamageModifier
{
    protected CombatEntity OwnerEntity;

    public virtual ResolveOrder Order => ResolveOrder.Default;

    public void SetOwner(CombatEntity ownerEntity) => OwnerEntity = ownerEntity;
    
    public abstract float Modify(CombatEntity otherEntity, float value);
}

public enum ResolveOrder
{
    Default,
    Final,
    Single
}