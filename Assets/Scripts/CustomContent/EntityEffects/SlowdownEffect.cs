using Combat;

public sealed class SlowdownEffect : EntityEffect
{
    private float _slowdownPerStack;
    private StatModifier _statModifier;

    public override EntityEffectType EffectType => EntityEffectType.Negative;
    public override bool CanBeApplied() => Entity.StatContainer.Has<Speed>();
    
    protected override void OnInitialized()
    {
        _slowdownPerStack = ArgumentsContainer.GetArgument<float>("SlowdownPerStack");
        _statModifier = new StatModifier(multiplier: _slowdownPerStack * Stack);
    }

    public override void Update()
    {
        _statModifier.SetMultiplier(_slowdownPerStack * Stack);
    }

    public override void ApplyToEntity()
    {
        Entity.StatContainer.Get<Speed>().AddStatModifier(_statModifier);
        Update();
    }

    public override void RemoveFromEntity()
    {
        Entity.StatContainer.Get<Speed>().RemoveStatModifier(_statModifier);
    }
}