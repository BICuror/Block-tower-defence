using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Reflection;
using CuroSettings;
using UnityEngine;

public sealed class ScreenSpaceAbmientOcclusionSettingApplier : SettingApplier<BoolSetting>
{
    private static BoolSetting _setting;
        
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        _setting = FetchSetting(SettingsEnum.Language, ApplyNewSettingValue);
        ApplyNewSettingValue();
    }
        
    private static void ApplyNewSettingValue()
    {
        RenderPipelineAsset asset = QualitySettings.GetRenderPipelineAssetAt(0);
        var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
        List<ScriptableRendererFeature> features = property.GetValue(asset) as List<ScriptableRendererFeature>;

        foreach (var feature in features)
        {
            if (feature is ScreenSpaceAmbientOcclusion)
            {
                
                feature.SetActive(_setting.Value);
            }
        }
    }
}