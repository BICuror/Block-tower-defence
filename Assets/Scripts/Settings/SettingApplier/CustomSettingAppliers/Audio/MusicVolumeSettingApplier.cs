using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public class MusicVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSetting("MusicVolume", ApplyNewSettingValue);
        }
        
        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetVolume(AudioChannelType.Music, Setting.Value);
        }
    }
}