using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public sealed class SFXVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        private static FloatSetting _setting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            _setting = FetchSetting(SettingsEnum.SFXVolume, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }

        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetVolume(AudioChannelType.SFX, _setting.Value);
        }
    }
}