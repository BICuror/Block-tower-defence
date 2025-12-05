using Cysharp.Threading.Tasks;

public sealed class MultishotEntityModificator : EntityModificator
{
    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<TaskCycle>();
    
    public override void Enable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskCycled += ActivateMultishot;
    }

    private async void ActivateMultishot()
    {
        float multishotDuration = Entity.StatContainer.Get<TaskRechargeDuration>().Value / 3;

        int shotCount = Args.GetArgument<int>("AdditionalShotsCount");
        
        float multishotStep = multishotDuration / shotCount;

        for (int i = 0; i < shotCount; i++)
        {
            await UniTask.WaitForSeconds(multishotStep);
            
            Entity.ComponentsContainer.Get<TaskCycle>().PerformTask();
        }
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<TaskCycle>().TaskCycled -= ActivateMultishot;
    }
}