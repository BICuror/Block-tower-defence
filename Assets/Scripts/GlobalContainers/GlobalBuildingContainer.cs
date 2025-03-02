using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Combat;

public sealed class GlobalBuildingContainer : MonoBehaviour
{
    private readonly List<BuildingEntity> _globalBuildingEntities = new();

    public IReadOnlyList<BuildingEntity> Entities => _globalBuildingEntities;
    
    public void Add(BuildingEntity buildingEntity)
    {
        if (_globalBuildingEntities.Contains(buildingEntity)) throw new DuplicateNameException();

        buildingEntity.ComponentsContainer.Get<BuildingHealth>().BuildingDestroyed += RemoveUponDestroyment;
        
        _globalBuildingEntities.Add(buildingEntity);
    }

    private void RemoveUponDestroyment(BuildingEntity buildingEntity)
    {
        buildingEntity.ComponentsContainer.Get<BuildingHealth>().BuildingDestroyed -= RemoveUponDestroyment;
        
        _globalBuildingEntities.Remove(buildingEntity);
    }
}