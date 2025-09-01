using Navigation;

public sealed class FearEffect : EntityEffect
{
    public override EntityEffectType EffectType => EntityEffectType.Negative;
    
    public override bool CanBeApplied() => Entity.ComponentsContainer.Has<NavigationAgent>();
    
    public override void ApplyToEntity()
    {
        Entity.ComponentsContainer.Get<NavigationAgent>().SetWeightPickLogic(NavigationAgentNodePicker.WeightPickType.Maximal);
    }

    public override void RemoveFromEntity()
    {
        Entity.ComponentsContainer.Get<NavigationAgent>().SetWeightPickLogic(NavigationAgentNodePicker.WeightPickType.Minimal);
    }
}