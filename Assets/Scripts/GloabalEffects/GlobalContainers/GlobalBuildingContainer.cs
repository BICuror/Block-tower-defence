using System;
using System.Collections.Generic;
using System.Data;
using Combat;

public sealed class GlobalBuildingContainer
{
    private readonly List<BuildingEntity> _globalBuildingEntities = new();

    public Action<BuildingEntity> BuildingAdded;
    public Action<BuildingEntity> BuildingRemoved;
    
    public IReadOnlyList<BuildingEntity> Entities => _globalBuildingEntities;

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