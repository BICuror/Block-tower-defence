using UnityEngine;
using Zenject;
using System;

public sealed class ItemModifierFactory : MonoBehaviour 
{
    [Inject] private DiContainer _diContainer;

    public GlobalToggleEffect CreateProperty(Type type) => CreateEffectInstance<GlobalToggleEffect>(type);

    public GlobalRewardEffect CreateReward(Type type) => CreateEffectInstance<GlobalRewardEffect>(type);

    private T CreateEffectInstance<T>(Type type)
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) Debug.LogError($"Reward type invalid {type.ToString()}");

        _diContainer.Inject(effect);

        return effect;
    }
}