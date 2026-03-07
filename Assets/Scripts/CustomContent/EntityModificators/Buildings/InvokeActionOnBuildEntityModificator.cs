using Cysharp.Threading.Tasks;

public sealed class InvokeActionOnBuildEntityModificator : EntityModificator
{
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += TryActivateTask;
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += StartBuildingProcess;
    }

    private async void TryActivateTask()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().PerformTask();
    }

    private void StartBuildingProcess()
    {
        if (Entity.ComponentsContainer.Get<DraggableObject>().IsPlaced)
        {
            Entity.ComponentsContainer.Get<IDraggable>().PickUp();
            Entity.ComponentsContainer.Get<IDraggable>().Place();
        }
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= TryActivateTask;
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= StartBuildingProcess;
    }
}