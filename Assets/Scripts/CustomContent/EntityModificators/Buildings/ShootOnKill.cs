using Combat;

public sealed class ShootOnKill : EntityModificator
{
    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<TaskCycle>();
    
    public override void Enable()
    {
        Entity.DamageModifierContainer.EntityKilled += InvokeActivity;
    }

    private void InvokeActivity(CombatEntity _)
    {
        Entity.ComponentsContainer.Get<TaskCycle>().PerformTask();
    }

    public override void Disable()
    {
        Entity.DamageModifierContainer.EntityKilled -= InvokeActivity;
    }
}