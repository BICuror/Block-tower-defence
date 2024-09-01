using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HealingTower : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _healAmount;
    public float HealAmount => _healAmount;

    [Header("Links")]
    [SerializeField] private BuildingHealthAreaScaner _buildingHealthAreaScaner;

    [SerializeField] private BeamSystem _beamSystem;

    private EntityHealth _currentBuilding;

    private BuildingTaskCycle _buildingTaskCycle;

    private Building _building;

    private void Start()
    {
        _buildingTaskCycle = GetComponent<BuildingTaskCycle>();
        _buildingTaskCycle.ShouldWorkDelegate = ShouldWork;
        _buildingTaskCycle.TaskPerformed.AddListener(HealBuilding);
        
        _buildingHealthAreaScaner.HealthComponentAdded.AddListener((EntityHealth health) => TrySetNewBuilding());
        _buildingHealthAreaScaner.HealthComponentRemoved.AddListener(TryRemoveBuilding);

        _buildingHealthAreaScaner.HealthComponentAdded.AddListener(SubscribeToBuilding);
        _buildingHealthAreaScaner.HealthComponentRemoved.AddListener(UnsubscribeToBuilding);

        _building = GetComponent<Building>();
        _building.PickedUp.AddListener(ClearBuilding);
        _building.BuildCompleted.RemoveListener(_buildingTaskCycle.StartCycle);
        _building.BuildCompleted.AddListener(TrySetNewBuilding);
    }

    private void ClearBuilding()
    {
        _beamSystem.DisableBeam();
        _currentBuilding = null;
    }

    private void SubscribeToBuilding(EntityHealth building) => building.Damaged.AddListener(TryToHealOnHurt);
    private void UnsubscribeToBuilding(EntityHealth building) => building.Damaged.RemoveListener(TryToHealOnHurt);

    private bool ShouldWork() 
    {
        if (_buildingHealthAreaScaner.IsEmpty()) return false;
        if (_building.IsBuilt() == false) return false;

        IReadOnlyList<EntityHealth> buildings = _buildingHealthAreaScaner.GetHealthComponentsList();

        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].GetHealthPrcentage() < 1f) return true; 
        }

        if (_currentBuilding != null) 
        {
            TryRemoveBuilding(_currentBuilding);
        }
        return false;
    }
    

    private void TrySetNewBuilding()
    {
        if (_currentBuilding == null && ShouldWork())
        {
            SetNewBuilding();
        }
    }

    private void TryRemoveBuilding(EntityHealth building)
    {
        if (_currentBuilding == (building as BuildingHealth))
        {
            ClearBuilding();

            if (ShouldWork())
            {
                SetNewBuilding();
            }
        }
    }

    private void TryToHealOnHurt()
    {
        if (_currentBuilding == null && _building.IsBuilt()) 
        {
            SetNewBuilding();       
        }
    }

    private void SetNewBuilding()
    {
        _currentBuilding = FindMostHurtBuilding();

        _beamSystem.SetTarget(_currentBuilding.transform);

        _buildingTaskCycle.StartCycle();
    }

    private EntityHealth FindMostHurtBuilding()
    {
        float leastHp = 1f;

        int index = 0;

        IReadOnlyList<EntityHealth> buildings = _buildingHealthAreaScaner.GetHealthComponentsList();

        for (int i = 0; i < buildings.Count; i++)
        {
            float buildingHealth = buildings[i].GetHealthPrcentage();

            if (buildingHealth < leastHp)
            {
                index = i;
                leastHp = buildingHealth;
            }
        }

        return buildings[index];
    }

    private void HealBuilding()
    {
        _currentBuilding.Heal(_healAmount);

        if (_currentBuilding.GetHealthPrcentage() == 1f)
        {
            TryRemoveBuilding(_currentBuilding);
        }
    }
}
