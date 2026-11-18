using TMPEffects.SerializedCollections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityEffectDataContainer", menuName = "EntityEffects/EntityEffectDataContainer")]

public sealed class EntityEffectDataContainer : ScriptableObject
{
    [SerializeField] private SerializedDictionary<string, EntityEffectConfig> _effectDatas = new();

    public EntityEffectConfig GetEffectData(Type effectType)
    {
        return _effectDatas[effectType.ToString()];
    }
}