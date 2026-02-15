using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class FullscreenSettingApplier : SettingApplier<EnumSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            FetchSetting(SettingsEnum.FullscreenMode, ApplyNewSettingValue);
        }
        
        private static void ApplyNewSettingValue()
        {
            Screen.fullScreenMode = Setting.GetValue<FullScreenMode>();
        }
    }
}