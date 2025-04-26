using Combat;

public sealed class ActivateWhenBuilt : EntityModificatior
{
    private BuildingEntity _ownerBuildingEntity;
    
    public override void Enable()
    {
        _ownerBuildingEntity = Entity as BuildingEntity;

        _ownerBuildingEntity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += TryActivateTask;
    }

    private void TryActivateTask()
    {
        if (_ownerBuildingEntity.ComponentsContainer.Has<TaskCycle>())
        {
            _ownerBuildingEntity.ComponentsContainer.Get<TaskCycle>().PerformTask();
        }
    }

    public override void Disable()
    {
        _ownerBuildingEntity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= TryActivateTask;
    }
}