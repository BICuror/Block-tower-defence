using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class FrameRateSettingApplier : SettingApplier<IntSetting>
    {
        private static IntSetting _setting; 
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            _setting = FetchSetting(SettingsEnum.FrameRate, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }
        
        private static void ApplyNewSettingValue()
        {
            Application.targetFrameRate = _setting.Value;
        }
    }
}