using UnityEngine;

public sealed class BuildingTaskCycle : TaskCycle
{
    private Building _building;

    private void Awake()
    {
        _building = GetComponent<Building>();
        
        _building.PickedUp.AddListener(StopCycle);

        _building.BuildCompleted += StartCycle;

        base.Awake();
    }

    public override bool CanWork() => _building.IsBuilt(); 
    
    private void OnDestroy() => _building.BuildCompleted -= StartCycle;
}
