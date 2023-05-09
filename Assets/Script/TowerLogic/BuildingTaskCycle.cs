using UnityEngine;

public sealed class BuildingTaskCycle : TaskCycle
{
    private Building _building;

    private void Start()
    {
        _building = GetComponent<Building>();
        
        _building.PickedUp.AddListener(StopCycle);

        _building.BuildCompleted.AddListener(StartSycle);
    }

    protected override bool CanWork() => _building.IsBuilt(); 
}
