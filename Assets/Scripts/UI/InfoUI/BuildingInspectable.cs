using UnityEngine;

public sealed class BuildingInspectable : InspectableObject
{
    [SerializeField] private Parameter[] _parametrs;

    public Parameter[] GetAllParameters() => _parametrs;
}
