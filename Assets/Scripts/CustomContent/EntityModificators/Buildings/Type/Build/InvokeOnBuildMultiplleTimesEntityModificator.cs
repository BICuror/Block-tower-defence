public sealed class InvokeOnBuildMultiplleTimesEntityModificator : EntityModificator
{
    private BuildingDraggable _buildingDraggable;
    private int _invokeTimes;
    private bool _enabled;
    
    public override void Enable()
    {
        _buildingDraggable = Entity.GetComponent<BuildingDraggable>();
        _invokeTimes = Args.GetArgument<int>("InvokeTimes");
        
        _enabled = true;
    }

    private void InvokeOnBuildEvent()
    {
        _buildingDraggable.BuildCompleted -= InvokeOnBuildEvent;
        
        for (int i = 0; i < _invokeTimes; i++)
        {
            _buildingDraggable.CompleteBuild();
        }
        
        if (_enabled) _buildingDraggable.BuildCompleted += InvokeOnBuildEvent;
    }

    public override void Disable()
    {
        _enabled = false;
        
        _buildingDraggable.BuildCompleted -= InvokeOnBuildEvent;
    }
}