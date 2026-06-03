using TMPEffects.SerializedCollections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DamageNumberConfig", menuName = "DamageNumberConfig")]

public sealed class DamageNumberConfig : ScriptableObject
{
    [SerializeField] private SerializedDictionary<DamageVisualsType, DamageTypeVisualsContainer> _damageTypeVisualContainers;

    public Color GetDamageColor(DamageVisualsType damageType) => _damageTypeVisualContainers[damageType].DamageColor;
}
    
[Serializable] public sealed class DamageTypeVisualsContainer
{
    public Color DamageColor;
}