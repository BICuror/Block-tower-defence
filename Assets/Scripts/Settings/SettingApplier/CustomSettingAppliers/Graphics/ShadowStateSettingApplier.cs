using CuroSettings;
using UnityEngine;

public sealed class ShadowStateSettingApplier : MonoBehaviour
{
    private BoolSetting _shadowStateSetting;
    private LightShadows _defaultShadows;
    private Light _light;
    
    private void Awake()
    {
        _light = GetComponent<Light>();
        _defaultShadows = _light.shadows;

        _shadowStateSetting = SettingsContainer.GetSetting<BoolSetting>(SettingsEnum.ShadowsEnabled);
        _shadowStateSetting.ValueChanged += UpdateShadows;

        UpdateShadows();
    }

    private void UpdateShadows()
    {
        if (_shadowStateSetting.Value) _light.shadows = _defaultShadows;
        else _light.shadows = LightShadows.None;
    }

    private void OnDestroy()
    {
        _shadowStateSetting.ValueChanged -= UpdateShadows;
    }
}