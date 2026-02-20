using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

public sealed class GlobalEffectFactory
{
    [Inject] private DiContainer _diContainer;

    public bool CanAppear(GlobalEffectData globalEffectData)
    {
        try
        {
            EffectApperanceCondition condition = (EffectApperanceCondition)Activator.CreateInstance(globalEffectData.EffectAppearanceCondition.ApperanceConditionType);
            
            condition.SetArgumentsContainer(globalEffectData.EffectAppearanceConditionArgumentsContainer); 
            _diContainer.Inject(condition);
            
            return condition.CanAppear();
        }
        catch (Exception e)
        {
            Debug.LogError($"Effect data has appearance condition, but doesn't have type {globalEffectData.name}");
            throw e;
        }
    }
    
    public List<GlobalEffect> CreateEffects(List<GlobalEffectData> effectDatas)
    {
        List<GlobalEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.AddRange(CreateEffects(effectData)));

        return resultEffectList;
    }
    
    public List<GlobalEffect> CreateEffects(GlobalEffectData globalEffectData)
    {
        List<GlobalEffect> effects = new();
        
        globalEffectData.InstanceItemTypeContainers.ForEach(instanceItemTypeContainer =>
        {
            effects.Add(CreateEffectInstance(globalEffectData, instanceItemTypeContainer.InstanceType));
        });
        
        return effects;
    } 
    
    private GlobalEffect CreateEffectInstance(GlobalEffectData effectData, Type type)
    {
        GlobalEffect effect = (GlobalEffect)Activator.CreateInstance(type);

        if (effect == null) throw new NullReferenceException($"Invalid effect type: {type}");

        _diContainer.Inject(effect);
        effect.SetArgumentsContainer(effectData.ArgumentsContainer); 
        effectData.Modify(effect);

        return effect;
    }
}