using Cysharp.Threading.Tasks;
using UnityEngine;
using Combat;

public sealed class CreateAOEOnKill : EntityModificator
{
    private bool _requireMarkedStatus;
    private AOE _aoePrefab;
    
    public override void Enable()
    {
        _requireMarkedStatus = Args.GetArgument<bool>("RequireMarkedStatus");
        _aoePrefab = Args.GetArgument<GameObject>("AOEPrefab").GetComponent<AOE>();

        Entity.ValueModifierContainer.EntityKilled += TryCreateAOE;
    }

    private void TryCreateAOE(CombatEntity entity)
    {
        if (_requireMarkedStatus && !entity.ComponentsContainer.Get<EntityEffectManager>().HasEffect(typeof(MarkEffect))) return;   
        
        CreateAOE(entity).Forget();
    }
    
    private async UniTask CreateAOE(CombatEntity entity)
    {
        AOE createdAOE = Object.Instantiate(_aoePrefab, entity.transform.position, Quaternion.identity);
        createdAOE.Initialize(Entity);
        await createdAOE.ActiveAOE();
        
        Object.Destroy(createdAOE.gameObject);
    }

    public override void Disable()
    {
        Entity.ValueModifierContainer.EntityKilled -= TryCreateAOE;
    }
}