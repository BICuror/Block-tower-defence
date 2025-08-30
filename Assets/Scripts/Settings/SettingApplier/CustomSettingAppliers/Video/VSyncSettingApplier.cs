using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class VSyncSettingApplier : SettingApplier<BoolSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSetting(SettingsEnum.VSync, ApplyNewSettingValue);
        }
        
        private static void ApplyNewSettingValue()
        {
            int settingValue = 0;

            if (Setting.Value) settingValue = 1;
            
            QualitySettings.vSyncCount = settingValue;
        }
    }
}