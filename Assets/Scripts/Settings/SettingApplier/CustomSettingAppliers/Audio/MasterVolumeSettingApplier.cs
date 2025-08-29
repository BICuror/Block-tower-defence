using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public class MasterVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSetting("MasterVolume", ApplyNewSettingValue);
        }
            
        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetMasterVolume(Setting.Value);
        }
    }
}