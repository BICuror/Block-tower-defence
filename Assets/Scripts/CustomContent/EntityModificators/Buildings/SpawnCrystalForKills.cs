using Zenject;
using Combat;

public sealed class SpawnCrystalForKills : EntityModificator
{
    [Inject] private ItemFactory _itemFactory;
    private int _killedEntities;
    
    public override void Enable()
    {
        Entity.ValueModifierContainer.EntityKilled += TrySpawnCrystal;
    }

    public override void Disable()
    {
        Entity.ValueModifierContainer.EntityKilled -= TrySpawnCrystal;
    }

    private void TrySpawnCrystal(CombatEntity killedEntity)
    { 
        _killedEntities++;
        
        if (_killedEntities >= Args.GetArgument<int>("KillPerCrystal"))
        {
            _killedEntities = 0;
            
            _itemFactory.CreateItem(1, Entity.transform.position);
        }
    }
}