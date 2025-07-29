public sealed class HealEffect : EntityTickEffect
{ 
    private float _healAmount;

    public override EntityEffectType EffectType => EntityEffectType.Positive;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _healAmount = ArgumentsContainer.GetArgument<float>("HealAmount");
    }
    
    protected override void Tick()
    {
        Entity.Health.ReceivePercentHeal(_healAmount);
    }
}