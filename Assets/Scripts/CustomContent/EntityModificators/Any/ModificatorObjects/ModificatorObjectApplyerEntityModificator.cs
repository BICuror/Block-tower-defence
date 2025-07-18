using UnityEngine;

public sealed class ModificatorObjectApplyerEntityModificator : EntityModificator
{
    private GameObject _instantiatedObjectModificator;
    
    public override void Enable()
    {
        GameObject prefab = Args.GetArgument<GameObject>("ObjectModificator");
        
        _instantiatedObjectModificator = Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().InstantiateAndAddModificator<GameObject>(prefab);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().DestroyModificator(_instantiatedObjectModificator);
    }
}