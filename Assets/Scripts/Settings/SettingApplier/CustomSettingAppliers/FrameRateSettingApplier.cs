using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class FrameRateSettingApplier : SettingApplier<IntSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSetting("FrameRate", ApplyNewSettingValue);
        }
        
        private static void ApplyNewSettingValue()
        {
            Application.targetFrameRate = Setting.Value;
        }
    }
}