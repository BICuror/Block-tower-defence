using UnityEngine;

public sealed class SquareAreaVisualisation : AreaVisualisation
{
    [SerializeField] private float _height = 100f;
    
    protected override Vector3 DisabledScale => new (0f, _height, 0f);
    protected override Vector3 EnabledScale => new (DefaultScale, _height, DefaultScale);
}