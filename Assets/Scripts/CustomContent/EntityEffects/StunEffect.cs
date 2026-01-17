using Navigation;

public sealed class StunEffect : EntityEffect
{
    public override EntityEffectType EffectType => EntityEffectType.Negative;

    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<TaskCycle>() || Entity.ComponentsContainer.Has<NavigationAgent>();
    
    public override void ApplyToEntity()
    {
        if (Entity.ComponentsContainer.Has<TaskCycle>())
        {
            Entity.ComponentsContainer.Get<TaskCycle>().CycleBlockTokenContainer.AddToken();
        }
        
        if (Entity.ComponentsContainer.Has<NavigationAgent>())
        {
            Entity.ComponentsContainer.Get<NavigationAgent>().Disable();
        }
    }

    public override void RemoveFromEntity()
    {
        if (Entity.ComponentsContainer.Has<TaskCycle>())
        {
            Entity.ComponentsContainer.Get<TaskCycle>().CycleBlockTokenContainer.RemoveToken();
        }
        
        if (Entity.ComponentsContainer.Has<NavigationAgent>())
        {
            Entity.ComponentsContainer.Get<NavigationAgent>().Enable();
        }
    }
}