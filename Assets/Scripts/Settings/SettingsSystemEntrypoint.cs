using UnityEngine;

namespace CuroSettings
{
    public sealed class SettingsSystemEntrypoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            SettingsConfig config = Resources.Load<SettingsConfig>("SettingsConfig");
            
            new SettingsContainer(new PlayerPrefsSettingsSaveLoader(), config.SettingConfigs);
        }
    }
}