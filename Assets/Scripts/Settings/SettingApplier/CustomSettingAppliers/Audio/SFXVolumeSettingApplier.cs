using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public class SFXVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSetting("SFXVolume", ApplyNewSettingValue);
        }

        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetVolume(AudioChannelType.SFX, Setting.Value);
        }
    }
}