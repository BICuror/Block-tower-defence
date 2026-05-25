using Cysharp.Threading.Tasks;
using Combat;

public sealed class ShootOnKill : EntityModificator
{
    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<TaskCycle>();
    
    public override void Enable()
    {
        Entity.ValueModifierContainer.EntityKilled += InvokeActivity;
    }

    private async void InvokeActivity(CombatEntity _)
    {
        await UniTask.WaitForFixedUpdate();
        
        Entity.ComponentsContainer.Get<TaskCycle>().PerformTask();
    }

    public override void Disable()
    {
        Entity.ValueModifierContainer.EntityKilled -= InvokeActivity;
    }
}