using Cysharp.Threading.Tasks;
using UnityEngine;

public class CreateAOEOnBuild : EntityModificator
{
    private bool _requireMarkedStatus;
    private AOE _aoePrefab;
    
    public override void Enable()
    {
        _aoePrefab = Args.GetArgument<GameObject>("AOEPrefab").GetComponent<AOE>();

        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += CreateAOESync;
    }

    private void CreateAOESync() => CreateAOE().Forget();
    
    private async UniTask CreateAOE()
    {
        AOE createdAOE = Object.Instantiate(_aoePrefab, Entity.transform.position, Quaternion.identity);
        createdAOE.Initialize(Entity);
        await createdAOE.ActiveAOE();
        
        Object.Destroy(createdAOE.gameObject);
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= CreateAOESync;
    }
}
