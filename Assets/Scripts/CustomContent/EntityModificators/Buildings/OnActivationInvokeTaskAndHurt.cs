public sealed class OnActivationInvokeTaskAndHurt : EntityModificator
{
    private float _activationDamage;
    
    public override void Enable()
    {
        _activationDamage = Args.GetArgument<float>("ActivationDamage");
        Entity.Activated += InvokeEntityTask;
    }

    private void InvokeEntityTask()
    {
        TaskCycle taskCycle = Entity.ComponentsContainer.Get<TaskCycle>();

        if (taskCycle.IsPossibleToPerformTask() && CombatUtilities.TryTakeNonLethalDamage(Entity, _activationDamage))
        {
            taskCycle.PerformTask();
        }
    }

    public override void Disable()
    {
        Entity.Activated -= InvokeEntityTask;
    }
}