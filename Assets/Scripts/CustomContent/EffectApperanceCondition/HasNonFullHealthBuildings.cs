using System.Collections.Generic;
using Zenject;
using Combat;

public sealed class HasNonFullHealthBuildings : EffectApperanceCondition
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    
    public override bool CanAppear()
    {
        IReadOnlyList<BuildingEntity> buildingEntities = _globalBuildingContainer.Entities;

        for (int i = 0; i < buildingEntities.Count; i++)
        {
            if (!buildingEntities[i].Health.IsFullHp()) return true;
        }

        return false;
    }
}