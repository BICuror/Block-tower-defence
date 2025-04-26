using UnityEngine;
using Zenject;
using System;

public sealed class EntityModificatorFactory
{
    [InjectLocal] private DiContainer _diContainer;
    
    public EntityModificatior CreateEntityModificationEffect(EntityModificatorData modificatorData)
    {
        EntityModificatior modificatior = CreateEffectInstance<EntityModificatior>(modificatorData.EffectType);
        modificatior.SetArgumentsContainer(modificatorData.ArgumentsContainer);
        return modificatior;
    } 
    
    private T CreateEffectInstance<T>(Type type)
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) throw new NullReferenceException($"Invalid effect type: {type}");

        _diContainer.Inject(effect);

        return effect;
    }
}