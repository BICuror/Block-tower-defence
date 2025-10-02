public sealed class HasteEffect : EntityEffect
{
    private StatModifier _statModifier = new();
    private float _rechargeUpPerStack;
    
    public override EntityEffectType EffectType => EntityEffectType.Positive;
    public override bool CanBeApplied() => Entity.StatContainer.Has<TaskRechargeDuration>();
    
    protected override void OnInitialized()
    {
        _rechargeUpPerStack = ArgumentsContainer.GetArgument<float>("RechargeSpeedIncrease");
    }

    public override void Update()
    {
        _statModifier.SetMultiplier(_rechargeUpPerStack * Stack);
    }

    public override void ApplyToEntity()
    {
        Entity.StatContainer.Get<TaskRechargeDuration>().AddStatModifier(_statModifier);
        
        Update();
    }

    public override void RemoveFromEntity()
    {
        Entity.StatContainer.Get<TaskRechargeDuration>().RemoveStatModifier(_statModifier);
    }
}