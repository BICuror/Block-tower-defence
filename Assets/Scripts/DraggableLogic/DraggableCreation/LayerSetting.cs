using UnityEngine;

[CreateAssetMenu(fileName = "LayerSetting", menuName = "PhysicLayers/LayerSetting")]

public sealed class LayerSetting : ScriptableObject 
{
    [SerializeField] private LayerMask _layerMask;
    
    public LayerMask GetLayerMask() => _layerMask;
}