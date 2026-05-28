using UnityEngine;

public sealed class LayerServiceInitializer : MonoBehaviour
{
    [SerializeField] private LayerSettingsServiceConfig _layerSettingsServiceConfig;
    
    private void Awake()
    {
        LayerService.SetLayerConfig(_layerSettingsServiceConfig);
    }
}