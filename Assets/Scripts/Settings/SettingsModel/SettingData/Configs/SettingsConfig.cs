using System.Collections.Generic;
using UnityEngine;

namespace CuroSettings
{
    [CreateAssetMenu(fileName = "SettingsConfig", menuName = "Settings/SettingsConfig")]
    
    public sealed class SettingsConfig : ScriptableObject
    {
        [SerializeField] private List<SettingConfig> _settingConfigs;

        public List<SettingConfig> SettingConfigs => new(_settingConfigs);
    }
}