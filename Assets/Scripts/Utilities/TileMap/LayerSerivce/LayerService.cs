using System.Collections.Generic;

public static class LayerService
{
    private static Dictionary<LayerSettingType, LayerSetting> _layerSettings;
    
    public static void SetLayerConfig(LayerSettingsServiceConfig config) => _layerSettings = config.LayerSettingDictionary;
    
    public static LayerSetting GetLayerSetting(LayerSettingType layerSettingType) => _layerSettings[layerSettingType];
}