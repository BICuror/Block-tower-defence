using Combat.Effects;
using Zenject;
using Combat;

public sealed class EffectFactory
{
    [InjectLocal] private DiContainer _diContainer;

    public bool EffectCanBeApplied(EntityModificatorData modificatorData)
    {
        TypedExecutor executor = new();
        
        ConditionalContainer container = ScriptableLogicFactory.CreateConditionalContainer(modificatorData.CanBeAppliedConditionContainer, new ScriptableLogicFactory.ScriptableLogicConstructorArgs());

        return executor.ExecuteConditions(container);
    }

    
    public bool EffectCanBeApplied(EntityModificatorData modificatorData, CombatEntity entity)
    {
        EntityTypedExecutor executor = new();

        ConditionalContainer container = ScriptableLogicFactory.CreateConditionalContainer(modificatorData.CanBeAppliedConditionContainer, new ScriptableLogicFactory.ScriptableLogicConstructorArgs());

        return executor.ExecuteConditions(container, entity);
    }
    
    public Effect CreateGlobalEffect(EntityModificatorData modificatorData, CombatEntity entity)
    {
        Domain domain = ScriptableLogicFactory.CreateDomain(modificatorData.DomainConfig);
        
        foreach (object injectableObject in domain.GetAllObjectsForInjection())
        {
            InjectObject(injectableObject);
        }

        Effect effect = new Effect(domain);

        return effect;

        void InjectObject(object injectableObject)
        {
            _diContainer.Inject(injectableObject);
        }
    }
    
    public Effect CreateEntityModificator(EntityModificatorData modificatorData, CombatEntity entity)
    {
        Domain domain = ScriptableLogicFactory.CreateDomain(modificatorData.DomainConfig, entity);
        
        foreach (object injectableObject in domain.GetAllObjectsForInjection())
        {
            InjectObject(injectableObject);
        }

        Effect effect = new Effect(domain);
        
        return effect;

        void InjectObject(object injectableObject)
        {
            _diContainer.Inject(injectableObject);
            entity.InjectCached(injectableObject);
        }
    }
}