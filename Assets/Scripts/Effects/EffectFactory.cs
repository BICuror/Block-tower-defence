using System.Collections.Generic;
using Zenject;
using System;

public sealed class EffectFactory
{
    [Inject] private DiContainer _diContainer;

    public bool GetAppearanceConditionValue(EffectData effectData)
    {
        EffectApperanceCondition condition = (EffectApperanceCondition)Activator.CreateInstance(effectData.EffectApperanceCondition.ApperanceConditionType);
        
        if (condition == null) throw new NullReferenceException($"Invalid condition type: {effectData.EffectApperanceCondition.ApperanceConditionType}");
        
        condition.SetArgumentsContainer(effectData.EffectApperanceCondition.ArgumentsContainer);
        _diContainer.Inject(condition);

        return condition.GetValue();
    }

    public ToggleEffect CreateToggleEffect(ToggleEffectData effectData)
    {
        ToggleEffect effect = CreateEffectInstance<ToggleEffect>(effectData.EffectType);
        effect.SetArgumentsContainer(effectData.ArgumentsContainer);
        return effect;
    } 

    public RewardEffect CreateRewardEffect(RewardEffectData effectData)
    {
        RewardEffect effect = CreateEffectInstance<RewardEffect>(effectData.EffectType);
        effect.SetArgumentsContainer(effectData.ArgumentsContainer);
        return effect;
    } 

    public EntityEffect CreateEntityEffect(EntityEffectData effectData)
    {
        EntityEffect effect = CreateEffectInstance<EntityEffect>(effectData.EffectType);
        effect.SetArgumentsContainer(effectData.ArgumentsContainer);
        return effect;
    } 
    
    private T CreateEffectInstance<T>(Type type)
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) throw new NullReferenceException($"Invalid effect type: {type}");

        _diContainer.Inject(effect);

        return effect;
    }

    public List<ToggleEffect> CreateToggleEffects(List<ToggleEffectData> effectDatas)
    {
        List<ToggleEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.Add(CreateToggleEffect(effectData)));

        return resultEffectList;
    }
    
    public List<RewardEffect> CreateRewardEffects(List<RewardEffectData> effectDatas)
    {
        List<RewardEffect> resultEffectList = new();
        
        effectDatas.ForEach(effectData => resultEffectList.Add(CreateRewardEffect(effectData)));

        return resultEffectList;
    }
}