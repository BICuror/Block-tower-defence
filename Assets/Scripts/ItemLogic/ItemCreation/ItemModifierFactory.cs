using UnityEngine;
using Zenject;
using System;

public sealed class ItemModifierFactory : MonoBehaviour 
{
    [Inject] private DiContainer _diContainer;

    public ToggleEffect CreateProperty(Type type) => CreateEffectInstance<ToggleEffect>(type);

    public RewardEffect CreateReward(Type type) => CreateEffectInstance<RewardEffect>(type);

    private T CreateEffectInstance<T>(Type type)
    {
        T effect = (T)Activator.CreateInstance(type);

        if (effect == null) Debug.LogError($"Reward type invalid {type.ToString()}");

        _diContainer.Inject(effect);

        return effect;
    }
}