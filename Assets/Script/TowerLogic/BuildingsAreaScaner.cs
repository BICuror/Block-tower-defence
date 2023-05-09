using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class BuildingsAreaScaner : MonoBehaviour
{
    private List<Building> _placedBuildings;

    private List<Building> _erectedBuildings;

    public UnityEvent<Building> ErectedBuildingAdded;

    public UnityEvent<Building> ErectedBuildingRemoved;
    
    private void Start()
    {
        _erectedBuildings = new List<Building>();

        _placedBuildings = new List<Building>();

        Building building = GetComponent<Building>();
    }
    
    public bool Empty() => _placedBuildings.Count == 0;

    public Building GetRandomBulding() => _placedBuildings[Random.Range(0, _placedBuildings.Count)];
    
    public List<Building> GetErectedBuildings() => new List<Building>(_erectedBuildings);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Building building) && other.gameObject != transform.parent.gameObject)
        {
            if (_placedBuildings.Contains(building) == false)
            {
                if (building.IsBuilt())
                {
                    AddPlacedBuilding(building);
                    AddErectedBuilding(building);
                }
                else if (building.IsPlaced())
                {
                    AddPlacedBuilding(building);
                }
                else 
                {
                    building.BuildingPlaced.AddListener(AddPlacedBuilding);
                }
            }
        }
    }

    private void AddPlacedBuilding(Building building)
    {
        if (_placedBuildings.Contains(building)) return;

        _placedBuildings.Add(building);

        building.gameObject.GetComponent<EntityHealth>().DestroyEvent.AddListener(RemoveFromList);

        building.BuildingBuilt.AddListener(AddErectedBuilding);

        building.BuildingPickedUp.AddListener(RemoveBuilding);

        building.BuildingPlaced.RemoveListener(AddPlacedBuilding);
    }
    
    private void AddErectedBuilding(Building building)
    {
        if (_erectedBuildings.Contains(building)) return;

        building.BuildingBuilt.RemoveListener(AddErectedBuilding);

        _erectedBuildings.Add(building);

        ErectedBuildingAdded?.Invoke(building);
    }

    private void RemoveBuilding(Building building)
    {
        if (_placedBuildings.Contains(building))
        {
            building.gameObject.GetComponent<EntityHealth>().DestroyEvent.RemoveListener(RemoveFromList);

            _placedBuildings.Remove(building);
        }
            
        if (_erectedBuildings.Contains(building)) 
        {
            _erectedBuildings.Remove(building);

            ErectedBuildingRemoved?.Invoke(building);
        }

        building.BuildingPickedUp.RemoveListener(RemoveBuilding);

        building.BuildingPlaced.AddListener(AddPlacedBuilding);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Building building))
        {   
            RemoveBuilding(building);

            building.BuildingPlaced.RemoveListener(AddPlacedBuilding);

            building.BuildingBuilt.RemoveListener(AddErectedBuilding);
        }
    }

    private void RemoveFromList(GameObject destroyedBuilding)
    {
        _placedBuildings.Remove(destroyedBuilding.GetComponent<Building>());

        ErectedBuildingRemoved.Invoke(destroyedBuilding.GetComponent<Building>());

        if (_erectedBuildings.Contains(destroyedBuilding.GetComponent<Building>())) 
        {
            _erectedBuildings.Remove(destroyedBuilding.GetComponent<Building>());
        }
    }

    private void OnDestroy() 
    {
        for (int i = 0; i < _erectedBuildings.Count; i++)
        {
            ErectedBuildingRemoved?.Invoke(_erectedBuildings[i]);
        }    
    }
}
