using Combat.Entity.Modificators;
using Combat;
using Combat.IngameUI.CombatEntityCanvas;

public abstract class EntityModificator
{
    protected EntityModificatorConfig ModificatorConfig;
    protected ArgumentsContainer Args;
    protected CombatEntity Entity;
    
    public void SetEntity(CombatEntity entity) => Entity = entity;
    public void SetArgumentsContainer(ArgumentsContainer argumentsContainer) => Args = argumentsContainer;
    public void SetEntityModificatorData(EntityModificatorConfig entityModificatorConfig) => ModificatorConfig = entityModificatorConfig;

    public virtual bool CanBeApplied() => true;

    protected EntityCanvasIcon AddIcon(bool hasValue, int value = 0, EntityCanvasIcon customIconPrefab = null)
    {
        return Entity.ComponentsContainer.Get<EntityCanvas>().AddIcon(ModificatorConfig.Icon, hasValue, value, customIconPrefab);
    }

    protected void RemoveIcon(EntityCanvasIcon icon)
    {
        Entity.ComponentsContainer.Get<EntityCanvas>().RemoveIcon(icon);
    }
    
    protected EntityCanvasAbilityIcon AddAbilityIcon(float value, EntityCanvasAbilityIcon customIconPrefab = null)
    {
        return Entity.ComponentsContainer.Get<EntityCanvas>().AddAbilityIcon(ModificatorConfig.Icon, value, customIconPrefab);
    }

    protected void RemoveAbilityIcon(EntityCanvasAbilityIcon icon)
    {
        Entity.ComponentsContainer.Get<EntityCanvas>().RemoveAbilityIcon(icon);
    }
    
    protected EntityCanvasBar AddBar(float value, EntityCanvasBar customBarPrefab = null)
    {
        return Entity.ComponentsContainer.Get<EntityCanvas>().AddBar(ModificatorConfig.Icon, value, customBarPrefab);
    }

    protected void RemoveBar(EntityCanvasBar bar)
    {
        Entity.ComponentsContainer.Get<EntityCanvas>().RemoveBar(bar);
    }
    
    public abstract void Enable();
    public abstract void Disable();
}