using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Combat;

public sealed class CreateHealingOrbsOnKill : EntityModificator
{
    [Inject] private DraggableCreator _draggableCreator;
    private DraggableObject _healingOrbPrefab;
    private Launcher _launcher;
    private int _requiredKills;
    private int _currentKills;
    
    public override void Enable()
    {
        _healingOrbPrefab = Args.GetArgument<GameObject>("HealingOrbPrefab").GetComponent<DraggableObject>();
        _launcher = Args.GetArgument<GameObject>("Launcher").GetComponent<Launcher>();
        _requiredKills = Args.GetArgument<int>("RequiredKills");
        
        Entity.ValueModifierContainer.EntityKilled += TrySpawnHealingOrb;
    }

    private void TrySpawnHealingOrb(CombatEntity killedEntity)
    {
        _currentKills++;

        if (_currentKills >= _requiredKills)
        {
            _currentKills = 0;
            _draggableCreator.CreateDraggableOnRandomPosition(_healingOrbPrefab, killedEntity.transform.position, 0, _launcher).Forget();
        }
    }
    
    public override void Disable()
    {
        Entity.ValueModifierContainer.EntityKilled -= TrySpawnHealingOrb;
    }
}
