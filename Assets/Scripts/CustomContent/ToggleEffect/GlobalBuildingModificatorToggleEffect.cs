using Combat.Entity.Modificators;
using Zenject;
using Combat;

public sealed class GlobalBuildingModificatorToggleEffect : GlobalEffect
{
    [Inject] private GlobalBuildingContainer _globalBuildingContainer;
    private EntityModificatorConfig _entityModificatorConfig;
    
    public override void Enable()
    {
        _entityModificatorConfig = Args.GetArgument<EntityModificatorConfig>("EntityModificatorConfig");
        
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
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AddModificator(_entityModificatorConfig);
    }

    private void RemoveEntityModificator(BuildingEntity entity)
    {
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().RemoveModificator(_entityModificatorConfig);
    }
}