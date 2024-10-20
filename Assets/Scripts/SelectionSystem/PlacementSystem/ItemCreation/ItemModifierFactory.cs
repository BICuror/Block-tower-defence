using UnityEngine;
using System;
using System.Collections.Generic;
using Zenject;

public sealed class ItemModifierFactory : MonoBehaviour 
{
    [Inject] private DiContainer _diContainer;

    public ItemProperty CreateProperty(Type type)
    {
        ItemProperty property = (ItemProperty)Activator.CreateInstance(type);

        if (property == null) Debug.LogError($"Property type invalid {type.ToString()}");

        _diContainer.Inject(property);

        return property;
    }

    public ItemReward CreateReward(Type type)
    {
        ItemReward reward = (ItemReward)Activator.CreateInstance(type);

        if (reward == null) Debug.LogError($"Reward type invalid {type.ToString()}");

        _diContainer.Inject(reward);

        return reward;
    }
}