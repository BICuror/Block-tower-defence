using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public class MusicVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        private static FloatSetting _setting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            _setting = FetchSetting(SettingsEnum.MusicVolume, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }
        
        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetVolume(AudioChannelType.Music, _setting.Value);
        }
    }
}