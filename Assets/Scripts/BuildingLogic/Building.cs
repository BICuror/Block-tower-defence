using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Cashing;

public sealed class Building : DraggableObject
{
    [Header("BuildingProcessSettings")]

    [Cached] private BuildTime _buildTime;

    private bool _isBuilt = true;
    
    public Action BuildCompleted;

    [HideInInspector] public UnityEvent<Building> BuildingPlaced;
    [HideInInspector] public UnityEvent<Building> BuildingBuilt;
    [HideInInspector] public UnityEvent<Building> BuildingPickedUp;
    
    public bool IsBuilt() => _isBuilt;

    private void Start()  
    {
        //_buildTime = this.GetStat<BuildTime>();

        Placed.AddListener(StartBuilding);

        PickedUp.AddListener(DisableBuilding);
        PickedUp.AddListener(StopBuildingProcess);
    }

    private void DisableBuilding() 
    {
        _isBuilt = false;

        BuildingPickedUp?.Invoke(this);
    }

    private void StartBuilding()
    {
        BuildingPlaced?.Invoke(this);

        StartCoroutine(StartBuildingProcess());
    }

    private IEnumerator StartBuildingProcess()
    {
        yield return new WaitForSeconds(_buildTime.Value);

        CompleteBuild();
    }

    private void CompleteBuild()
    {
        _isBuilt = true;

        BuildCompleted?.Invoke();

        BuildingBuilt?.Invoke(this);
    }

    private void StopBuildingProcess() => StopAllCoroutines();
}
