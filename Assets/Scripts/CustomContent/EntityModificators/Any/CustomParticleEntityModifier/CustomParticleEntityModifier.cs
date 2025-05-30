using Combat;

public sealed class CustomParticleEntityModifier : EntityModificator
{
    private EntityEffectParticleHandler _entityEffectParticleHandler;
    
    public override void Enable()
    {
        _entityEffectParticleHandler = Entity.ComponentsContainer.Get<ParticleEffectManager>().ApplyCustomEffect(Args.GetArgument<EntityEffectParticleHandler>("EntityEffectParticleHandler"));
    }

    public override void Disable()
    {
        Entity.ComponentsContainer.Get<ParticleEffectManager>().DestroyCustomEffect(_entityEffectParticleHandler);
    }
}