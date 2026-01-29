using System.Collections.Generic;
using System.Data;
using System;
using Combat;

public sealed class GlobalBuildingContainer
{
    private readonly List<BuildingEntity> _globalBuildingEntities = new();

    public Action<BuildingEntity> BuildingAdded;
    public Action<BuildingEntity> BuildingRemoved;
    
    public IReadOnlyList<BuildingEntity> Entities => _globalBuildingEntities;

    public List<BuildingEntity> GetPlayerBuildings()
    {
        return _globalBuildingEntities.FindAll(buildingEntity => buildingEntity.ComponentsContainer.Get<EntityModificatorsContainer>().AvailableModificators.Count > 0);
    }
    
    public List<EntityModifcatorTag> GetBuildingTags(BuildingEntity excludedEntity = null)
    {
        List<BuildingEntity> includedBuildings = new List<BuildingEntity>(_globalBuildingEntities);
        
        if (excludedEntity) includedBuildings.Remove(excludedEntity);
        
        List<EntityModifcatorTag> resultTags = new();
        
        includedBuildings.ForEach(building =>
        {
            resultTags.AddRange(building.ComponentsContainer.Get<EntityModificatorsContainer>().GetAppliedTags());
        });
        
        return resultTags;
    }
    
    public void Add(BuildingEntity buildingEntity)
    {
        if (_globalBuildingEntities.Contains(buildingEntity)) throw new DuplicateNameException();

        buildingEntity.BuildingHealth.BuildingDestroyed += RemoveUponDestroyment;
        
        _globalBuildingEntities.Add(buildingEntity);
        
        BuildingAdded?.Invoke(buildingEntity);
    }

    private void RemoveUponDestroyment(BuildingEntity buildingEntity)
    {
        buildingEntity.BuildingHealth.BuildingDestroyed -= RemoveUponDestroyment;
        
        _globalBuildingEntities.Remove(buildingEntity);
        
        BuildingRemoved?.Invoke(buildingEntity);
    }
}