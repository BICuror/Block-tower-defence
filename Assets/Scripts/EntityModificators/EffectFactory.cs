using Combat.Entity.Modificators;
using ScriptableLogic.Conditions;
using Combat.Effects;
using Zenject;
using Combat;

public sealed class EffectFactory
{
    [InjectLocal] private DiContainer _diContainer;

    public bool EffectCanBeApplied(EntityModificatorConfig modificatorConfig)
    {
        TypedExecutor executor = new();
        
        ConditionalContainer container = ScriptableLogicFactory.CreateConditionalContainer(modificatorConfig.CanBeAppliedConditionContainer, new ScriptableLogicFactory.ScriptableLogicConstructorArgs());

        return executor.ExecuteConditions(container);
    }
    
    public bool EffectCanBeApplied(EntityModificatorConfig modificatorConfig, CombatEntity entity)
    {
        EntityTypedExecutor executor = new();

         ConditionalContainer container = ScriptableLogicFactory.CreateConditionalContainer(modificatorConfig.CanBeAppliedConditionContainer, new ScriptableLogicFactory.ScriptableLogicConstructorArgs());

        return executor.ExecuteConditions(container, entity);
    }
    
    public Effect CreateGlobalEffect(EntityModificatorConfig modificatorConfig, CombatEntity entity)
    {
        Domain domain = ScriptableLogicFactory.CreateDomain(modificatorConfig.DomainConfig);
        
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
    
    public Effect CreateEntityModificator(EntityModificatorConfig modificatorConfig, CombatEntity entity)
    {
        Domain domain = ScriptableLogicFactory.CreateDomain(modificatorConfig.DomainConfig, entity);
        
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