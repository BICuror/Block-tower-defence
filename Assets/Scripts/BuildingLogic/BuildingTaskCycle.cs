using Cashing;

public sealed class BuildingTaskCycle : TaskCycle
{
    [Cached] private BuildingDraggable _buildingDraggable;

    public BuildingTaskCycle()
    {
        _buildingDraggable.PickedUp += StopRechargeProcess;
        _buildingDraggable.BuildCompleted += StartCycle;
    }

    protected override bool CanWork() => _buildingDraggable.IsBuilt; 
    
    private void OnDestroy() => _buildingDraggable.BuildCompleted -= StartCycle;
}