using UnityEngine;

public sealed class SphereAreaVisualisation : AreaVisualisation
{
    protected override Vector3 DisabledScale => Vector3.zero;
    protected override Vector3 EnabledScale => new Vector3(DefaultScale, DefaultScale, DefaultScale);
}