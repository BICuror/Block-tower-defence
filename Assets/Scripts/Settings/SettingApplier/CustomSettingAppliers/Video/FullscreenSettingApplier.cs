using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class FullscreenSettingApplier : SettingApplier<EnumSetting>
    {
        private static EnumSetting _setting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _setting = FetchSetting(SettingsEnum.FullscreenMode, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }
        
        private static void ApplyNewSettingValue()
        {
            Screen.SetResolution(Screen.width, Screen.height, _setting.GetValue<FullScreenMode>());
        }
    }
}