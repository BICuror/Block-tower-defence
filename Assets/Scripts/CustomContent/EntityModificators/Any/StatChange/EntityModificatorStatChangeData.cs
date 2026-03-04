using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EntityModificatorData", menuName = "EntityModificators/EntityStatChangeModificatorData")]

public sealed class EntityModificatorStatChangeData : EntityModificatorData {}

[Serializable]
public sealed class StatChange
{
    public StatData StatData;
    public float FlatChange;
    public float MultiplierChange;
}