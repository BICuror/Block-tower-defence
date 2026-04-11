using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class VSyncSettingApplier : SettingApplier<BoolSetting>
    {
        private static BoolSetting _setting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            _setting = FetchSetting(SettingsEnum.VSync, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }
        
        private static void ApplyNewSettingValue()
        {
            int settingValue = 0;

            if (_setting.Value) settingValue = 1;
            
            QualitySettings.vSyncCount = settingValue;
        }
    }
}