using System.Collections.Generic;
using Zenject;
using System;

public sealed class GlobalEffectFactory
{
    [Inject] private DiContainer _diContainer;

    public bool CanAppear(GlobalEffectData globalEffectData)
    {
        EffectApperanceCondition condition = (EffectApperanceCondition)Activator.CreateInstance(globalEffectData.EffectAppearanceCondition.ApperanceConditionType);
        
        if (condition == null) throw new NullReferenceException($"Invalid condition type: {globalEffectData.EffectAppearanceCondition.ApperanceConditionType}");
        
        condition.SetArgumentsContainer(globalEffectData.EffectAppearanceCondition.ArgumentsContainer);
        _diContainer.Inject(condition);

        return condition.CanAppear();
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

    public List<GlobalRewardEffect> CreateRewardEffects(RewardGlobalEffectData globalEffectData)
    {
        List<GlobalRewardEffect> rewardEffects = new();

        globalEffectData.InstanceItemTypeContainers.ForEach(instanceItemTypeContainer =>
        {
            rewardEffects.Add(CreateEffectInstance<GlobalRewardEffect>(globalEffectData, instanceItemTypeContainer.InstanceType));
        });
        
        return rewardEffects;
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
    
    public List<GlobalRewardEffect> CreateRewardEffects(List<RewardGlobalEffectData> effectDatas)
    {
        List<GlobalRewardEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.AddRange(CreateRewardEffects(effectData)));

        return resultEffectList;
    }
}