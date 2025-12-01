using Cashing;

public sealed class BuildingTaskCycle : TaskCycle
{
    [Cached] private BuildingDraggable _buildingDraggable;

    private void Start()
    {
        _buildingDraggable.PickedUp += StopRechargeProcess;
        _buildingDraggable.BuildCompleted += TryCycle;
    }

    protected override bool CanWork() => _buildingDraggable.IsBuilt;

    private void OnDestroy()
    {
        base.OnDestroy();
        _buildingDraggable.BuildCompleted -= TryCycle;
    } 
}