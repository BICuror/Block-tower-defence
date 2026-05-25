public sealed class InvokeActionOnBuildEntityModificator : EntityModificator
{
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += TryActivateTask;
    }

    private void TryActivateTask()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().PerformTask();
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= TryActivateTask;
    }
}