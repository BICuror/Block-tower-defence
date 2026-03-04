using UnityEngine;

public class GameObjectApplierEntityModificator : EntityModificator
{
    private GameObject _instantiatedObject;
    
    public override void Enable()
    {
        GameObject prefab = Args.GetArgument<GameObject>("ObjectPrefab");
        
        _instantiatedObject = Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().InstantiateAndAddModificator(prefab);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<EntityObjectModificatorContainer>().RemoveAndDestroyModificator(_instantiatedObject);
    }
}