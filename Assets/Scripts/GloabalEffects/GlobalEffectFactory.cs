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

    public List<GlobalToggleEffect> CreateToggleEffects(ToggleGlobalEffectData globalEffectData)
    {
        List<GlobalToggleEffect> toggleEffects = new();
        
        globalEffectData.InstanceItemTypeContainers.ForEach(instanceItemTypeContainer =>
        {
            toggleEffects.Add(CreateEffectInstance<GlobalToggleEffect>(globalEffectData, instanceItemTypeContainer.InstanceType));
        });
        
        return toggleEffects;
    } 
    
    private T CreateEffectInstance<T>(GlobalEffectData effectData, Type type) where T : GlobalEffect
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) throw new NullReferenceException($"Invalid effect type: {type}");

        _diContainer.Inject(effect);
        effect.SetArgumentsContainer(effectData.ArgumentsContainer); 
        effectData.Modify(effect);

        return effect;
    }

    public List<GlobalToggleEffect> CreateToggleEffects(List<ToggleGlobalEffectData> effectDatas)
    {
        List<GlobalToggleEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.AddRange(CreateToggleEffects(effectData)));

        return resultEffectList;
    }
}