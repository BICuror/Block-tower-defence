using Combat.Entity.Modificators;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "EntityModificatorConfig", menuName = "EntityModificators/EntityStatChangeModificatorData")]

public sealed class EntityModificatorStatChangeConfig : EntityModificatorConfig {}

[Serializable]
public sealed class StatChange
{
    [FormerlySerializedAs("StatData")] public StatConfig _statConfig;
    public float FlatChange;
    public float MultiplierChange;
}