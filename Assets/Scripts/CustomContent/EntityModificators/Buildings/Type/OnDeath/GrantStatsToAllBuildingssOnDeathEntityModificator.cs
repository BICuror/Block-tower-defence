using Zenject;
using Combat;

public sealed class GrantStatsToAllBuildingssOnDeathEntityModificator : EntityModificator
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    
    public override void Enable()
    {
        Entity.Health.Died += GrantStatsToAllBuildings;
    }

    private void GrantStatsToAllBuildings()
    {
        foreach (BuildingEntity buildingEntity in _globalBuildingContainer.Entities)
        {
            if (buildingEntity != Entity)
            {
                ModificatorData.StatChanges.ForEach(statChange =>
                {
                    Stat stat = buildingEntity.StatContainer.Get(statChange.StatData.GetStatType());
                    
                    stat.ChangeFlat(statChange.FlatChange);
                    stat.ChangeMultiplier(statChange.MultiplierChange);
                });
            }
        }
    }
    
    public override void Disable()
    {
        Entity.Health.Died -= GrantStatsToAllBuildings;
    }
}