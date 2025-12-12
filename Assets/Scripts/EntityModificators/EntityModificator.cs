using Combat;

public abstract class EntityModificator
{
    private EntityModificatorData _entityModificatorData;
    protected ArgumentsContainer Args;
    protected CombatEntity Entity;
    
    public void SetEntity(CombatEntity entity) => Entity = entity;
    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public void SetEntityModificatorData(EntityModificatorData entityModificatorData) => _entityModificatorData = entityModificatorData;

    public virtual bool CanBeApplied() => true;

    protected EntityCanvasIcon AddIcon(bool hasValue, int value = 0, EntityCanvasIcon customIconPrefab = null)
    {
        return Entity.ComponentsContainer.Get<EntityCanvas>().AddIcon(_entityModificatorData.Icon, hasValue, value, customIconPrefab);
    }

    protected void RemoveIcon(EntityCanvasIcon icon)
    {
        Entity.ComponentsContainer.Get<EntityCanvas>().RemoveIcon(icon);
    }
    
    protected EntityCanvasBar AddBar(float value, EntityCanvasBar customBarPrefab = null)
    {
        return Entity.ComponentsContainer.Get<EntityCanvas>().AddBar(_entityModificatorData.Icon, value, customBarPrefab);
    }

    protected void RemoveBar(EntityCanvasBar bar)
    {
        Entity.ComponentsContainer.Get<EntityCanvas>().RemoveBar(bar);
    }
    
    public abstract void Enable();
    public abstract void Disable();
}