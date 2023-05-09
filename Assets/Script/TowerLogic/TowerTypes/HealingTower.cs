using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HealingTower : MonoBehaviour
{
    [SerializeField] private BuildingsAreaScaner _buildingAreaScaner;

    private TaskCycle _taskCycle;

    private EntityHealth _buildingToHeal;

    private Building _erectableBuilding;

    [SerializeField] private LineRenderer _lineRenderer;
 
    [SerializeField] private float _healSpeed;

    public void Work()
    {
        _buildingToHeal.Heal(_healSpeed);
    }

    private void Start()
    {
        _taskCycle = GetComponent<TaskCycle>();
        _erectableBuilding = GetComponent<Building>();
        _taskCycle.ShouldWorkDelegate = LOL;

        _buildingAreaScaner.ErectedBuildingAdded.AddListener(AddBuilding);
        _buildingAreaScaner.ErectedBuildingRemoved.AddListener(RemoveBuilding);
    }

    private void AddBuilding(Building building)
    {
        building.gameObject.GetComponent<EntityHealth>().HealthChanged.AddListener(TryToFindBuildingToHeal);

        if (_buildingToHeal == null || building.gameObject.GetComponent<EntityHealth>().GetHealthPrcentage() < _buildingToHeal.GetHealthPrcentage())
        {
            TryToFindBuildingToHeal();
        }
    }

    private void RemoveBuilding(Building building)
    {
        building.gameObject.GetComponent<EntityHealth>().HealthChanged.RemoveListener(TryToFindBuildingToHeal);

        if (building.gameObject == _buildingToHeal.gameObject)
        {
            TryToFindBuildingToHeal();
        }
    }

    public bool LOL() => _buildingToHeal != null && _erectableBuilding.IsBuilt();

    private void TryToFindBuildingToHeal()
    {
        float leastHp = 1f;

        int index = 0;

        List<Building> buildings = _buildingAreaScaner.GetErectedBuildings();

        for (int i = 0; i < buildings.Count; i++)
        {
            float buildingHp = buildings[i].gameObject.GetComponent<EntityHealth>().GetHealthPrcentage();

            if (buildingHp < leastHp)
            {
                leastHp = buildingHp;

                index = i;
            }
        }

        if (leastHp < 1) // found something
        {
            _buildingToHeal = buildings[index].gameObject.GetComponent<EntityHealth>();

            _lineRenderer.SetPositions(new Vector3[2]{transform.position, _buildingToHeal.gameObject.transform.position});

            _taskCycle.StartSycle();
        }
        else 
        {
            _buildingToHeal = null;

            _lineRenderer.SetPositions(new Vector3[2]{transform.position, transform.position});

            _taskCycle.StopCycle();
        }
    }
}
