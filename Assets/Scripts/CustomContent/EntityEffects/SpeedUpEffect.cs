public sealed class SpeedUpEffect : EntityEffect
{
    private float _speedMultiplier;
    private StatModifier _statModifier;
    
    public override EntityEffectType EffectType => EntityEffectType.Positive;

    public override bool CanBeApplied() => Entity.StatContainer.Has<Speed>();
    
    protected override void OnInitialized()
    {
        _speedMultiplier = ArgumentsContainer.GetArgument<float>("SpeedMultiplier");
        _statModifier = new StatModifier(0f, _speedMultiplier);
    }
    
    public override void ApplyToEntity()
    {
        Entity.StatContainer.Get<Speed>().AddStatModifier(_statModifier);
    }

    public override void RemoveFromEntity()
    {
        Entity.StatContainer.Get<Speed>().RemoveStatModifier(_statModifier);
    }
}