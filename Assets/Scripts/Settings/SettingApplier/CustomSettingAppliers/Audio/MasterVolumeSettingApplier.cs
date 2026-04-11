using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public class MasterVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        private static FloatSetting _setting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            _setting = FetchSetting(SettingsEnum.MasterVolume, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }
            
        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetMasterVolume(_setting.Value);
        }
    }
}