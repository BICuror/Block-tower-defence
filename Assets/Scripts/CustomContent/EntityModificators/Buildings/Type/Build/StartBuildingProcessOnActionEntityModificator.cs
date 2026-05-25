public class StartBuildingProcessOnActionEntityModificator : EntityModificator
{
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += StartBuildingProcess;
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
        Entity.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= StartBuildingProcess;
    }
}