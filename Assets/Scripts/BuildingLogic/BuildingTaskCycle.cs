using UnityEngine;

public sealed class BuildingTaskCycle : TaskCycle
{
    private Building _building;

    private void Awake()
    {
        _building = GetComponent<Building>();
        
        _building.PickedUp.AddListener(StopCycle);

        _building.BuildCompleted.AddListener(StartCycle);

        base.Awake();
    }

    public override bool CanWork() => _building.IsBuilt(); 
}
