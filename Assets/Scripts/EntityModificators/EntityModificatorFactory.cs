using Zenject;
using System;

public sealed class EntityModificatorFactory
{
    [InjectLocal] private DiContainer _diContainer;
    
    public EntityModificator CreateEntityModificationEffect(EntityModificatorData modificatorData)
    {
        EntityModificator modificator = CreateEffectInstance<EntityModificator>(modificatorData.ModificatorInstanceType);
        modificator.SetArgumentsContainer(modificatorData.ArgumentsContainer);
        modificatorData.Modify(modificator);
        return modificator;
    } 
    
    private T CreateEffectInstance<T>(Type type)
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) throw new NullReferenceException($"Invalid effect type: {type}");

        _diContainer.Inject(effect);
        
        return effect;
    }
}