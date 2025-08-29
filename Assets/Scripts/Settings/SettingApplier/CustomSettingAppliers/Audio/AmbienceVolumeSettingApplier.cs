using UnityEngine;
using CuroAudio;

namespace CuroSettings.CustomSettingAppliers.Audio
{
    public class AmbienceVolumeSettingApplier : SettingApplier<FloatSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            FetchSetting("AmbienceVolume", ApplyNewSettingValue);
        }

        private static void ApplyNewSettingValue()
        {
            AudioSystem.SetVolume(AudioChannelType.Ambience, Setting.Value);
        }
    }
}