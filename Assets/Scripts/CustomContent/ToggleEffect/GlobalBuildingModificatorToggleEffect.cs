using Zenject;
using Combat;
using UnityEngine;

public sealed class GlobalBuildingModificatorToggleEffect : GlobalToggleEffect
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    private EntityModificatorData _entityModificatorData;
    
    public override void Enable()
    {
        _entityModificatorData = Args.GetArgument<EntityModificatorData>("EntityModificatorData");
        
        _globalBuildingContainer.BuildingAdded += AddEntityModificator;
        _globalBuildingContainer.BuildingRemoved += RemoveEntityModificator;
        
        foreach (BuildingEntity buildingEntity in _globalBuildingContainer.Entities)
        {
            AddEntityModificator(buildingEntity);
        }
    }

    public override void Disable()
    {
        _globalBuildingContainer.BuildingAdded -= AddEntityModificator;
        _globalBuildingContainer.BuildingRemoved -= RemoveEntityModificator;
        
        foreach (BuildingEntity buildingEntity in _globalBuildingContainer.Entities)
        {
            RemoveEntityModificator(buildingEntity);
        }
    }

    private void AddEntityModificator(BuildingEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddEffect(_entityModificatorData);
    }

    private void RemoveEntityModificator(BuildingEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveEffect(_entityModificatorData);
    }
}