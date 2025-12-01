using Combat;

public abstract class EntityModificator
{
    protected ArgumentsContainer Args;
    protected CombatEntity Entity;
    
    public void SetEntity(CombatEntity entity) => Entity = entity;
    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;

    public virtual bool CanBeApplied() => true;
    
    public abstract void Enable();
    public abstract void Disable();
}