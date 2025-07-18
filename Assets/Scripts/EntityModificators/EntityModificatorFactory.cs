using System.Collections.Generic;
using Zenject;
using System;

public sealed class EntityModificatorFactory
{
    [InjectLocal] private DiContainer _diContainer;
    
    public List<EntityModificator> CreateEntityModificators(EntityModificatorData modificatorData)
    {
        List<EntityModificator> modificators = new();
        
        modificatorData.ItemTypeContainers.ForEach(itemTypeContainer =>
        {
            modificators.Add(CreateEntityModificator(modificatorData, itemTypeContainer.InstanceType));
        });
        
        return modificators;
    }

    private EntityModificator CreateEntityModificator(EntityModificatorData modificatorData, Type instanceType)
    {
        EntityModificator modificator = CreateEffectInstance<EntityModificator>(instanceType);
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