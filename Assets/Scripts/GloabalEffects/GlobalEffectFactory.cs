using System.Collections.Generic;
using Zenject;
using System;

public sealed class GlobalEffectFactory
{
    [Inject] private DiContainer _diContainer;

    public bool GetAppearanceConditionValue(GlobalEffectData globalEffectData)
    {
        EffectApperanceCondition condition = (EffectApperanceCondition)Activator.CreateInstance(globalEffectData.EffectApperanceCondition.ApperanceConditionType);
        
        if (condition == null) throw new NullReferenceException($"Invalid condition type: {globalEffectData.EffectApperanceCondition.ApperanceConditionType}");
        
        condition.SetArgumentsContainer(globalEffectData.EffectApperanceCondition.ArgumentsContainer);
        _diContainer.Inject(condition);

        return condition.GetValue();
    }

    public GlobalToggleEffect CreateToggleEffect(ToggleGlobalEffectData globalEffectData)
    {
        GlobalToggleEffect effect = CreateEffectInstance<GlobalToggleEffect>(globalEffectData.EffectType);
        effect.SetArgumentsContainer(globalEffectData.ArgumentsContainer);
        return effect;
    } 

    public GlobalRewardEffect CreateRewardEffect(RewardGlobalEffectData globalEffectData)
    {
        GlobalRewardEffect effect = CreateEffectInstance<GlobalRewardEffect>(globalEffectData.EffectType);
        effect.SetArgumentsContainer(globalEffectData.ArgumentsContainer);
        return effect;
    } 
    
    private T CreateEffectInstance<T>(Type type)
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) throw new NullReferenceException($"Invalid effect type: {type}");

        _diContainer.Inject(effect);

        return effect;
    }

    public List<GlobalToggleEffect> CreateToggleEffects(List<ToggleGlobalEffectData> effectDatas)
    {
        List<GlobalToggleEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.Add(CreateToggleEffect(effectData)));

        return resultEffectList;
    }
    
    public List<GlobalRewardEffect> CreateRewardEffects(List<RewardGlobalEffectData> effectDatas)
    {
        List<GlobalRewardEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.Add(CreateRewardEffect(effectData)));

        return resultEffectList;
    }
}