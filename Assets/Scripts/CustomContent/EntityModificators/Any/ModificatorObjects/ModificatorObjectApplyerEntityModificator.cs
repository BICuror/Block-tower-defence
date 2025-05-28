using UnityEngine;

public sealed class ModificatorObjectApplyerEntityModificator : EntityModificator
{
    private GameObject _instantiatedObjectModificator;
    
    public override void Enable()
    {
        _instantiatedObjectModificator = Object.Instantiate(Args.GetArgument<GameObject>("ObjectModificator"));
        
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().AddModificator(_instantiatedObjectModificator);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().DestroyModificator(_instantiatedObjectModificator);
    }
}