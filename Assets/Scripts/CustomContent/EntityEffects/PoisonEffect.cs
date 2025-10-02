public sealed class PoisonEffect : EntityTickEffect
{
    private float _damagePerStack;
    private float _healthThreshold;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _damagePerStack = ArgumentsContainer.GetArgument<float>("DamagePerStack");
        _healthThreshold = ArgumentsContainer.GetArgument<float>("HealthThreshold");
    }
    
    protected override void Tick()
    {
        if (Entity.Health.GetHp() <= _healthThreshold) return;
        
        float tickDamage = _damagePerStack * Stack;
        
        if (Entity.Health.GetHp() <= tickDamage)
        {
            Entity.Health.ReceiveEffectDamage(Entity.Health.GetHp() - _healthThreshold);
        }
        else
        {
            Entity.Health.ReceiveEffectDamage(tickDamage);
        }
    }

    public override EntityEffectType EffectType => EntityEffectType.Negative;
}