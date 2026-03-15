public sealed class ModificatorObjectApplyerEntityModificator : EntityModificator
{
    private EntityObjectModifier _instantiatedObjectModificator;
    
    public override bool CanBeApplied()
    {
        EntityObjectModifier prefab = Args.GetArgument<EntityObjectModifier>("ObjectModificator");

        return Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().CanBeAppliedToEntity(prefab);
    }
    
    public override void Enable()
    {
        EntityObjectModifier prefab = Args.GetArgument<EntityObjectModifier>("ObjectModificator");
        
        _instantiatedObjectModificator = Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().InstantiateAndAddModificator(prefab, Args);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().RemoveAndDestroyModificator(_instantiatedObjectModificator);
    }
}