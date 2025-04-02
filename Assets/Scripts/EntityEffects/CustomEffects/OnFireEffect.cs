public sealed class OnFireEffect : EntityTickEffect
{
    private float _damagePerStack;
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _damagePerStack = ArgumentsContainer.GetArgument<float>("DamagePerStack");
    }
    
    protected override void Tick()
    {
        Entity.Health.ReceiveEffectDamage(_damagePerStack * Stack);
    } 
}