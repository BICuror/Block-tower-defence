using System.Collections.Generic;
using System;

namespace CuroSettings
{
    public sealed class SettingsContainer
    {
        private static ISettingsSaveLoader _settingsSaveLoader;
        private static Dictionary<SettingsEnum, Setting> _settings;

        public SettingsContainer(ISettingsSaveLoader settingsSaveLoader, List<SettingConfig> settingConfigs)
        {
            _settingsSaveLoader = settingsSaveLoader;
            
            InitializeSettings(settingConfigs);
        }
        
        public static bool SettingsExists(SettingsEnum key) => _settings.ContainsKey(key);
        
        public static T GetSetting<T>(SettingsEnum key) where T : Setting
        {
            return (T)_settings[key];
        }
        
        public static void SaveAll()
        {
            foreach (Setting setting in _settings.Values)
            {
                setting.Save();
            }
        }

        public static void LoadAll()
        {
            foreach (Setting setting in _settings.Values)
            {
                setting.Load();
            }
        }
        
        private void InitializeSettings(List<SettingConfig> settingConfigs)
        {
            _settings = new();

            settingConfigs.ForEach(config =>
            {
                _settings.Add(config.SettingEnum, CreateSetting(config));
            });
        }

        private Setting CreateSetting(SettingConfig settingConfig)
        {
            Setting setting;

            switch (settingConfig.Type)
            {
                case SettingType.Float: setting = new FloatSetting(_settingsSaveLoader, settingConfig.SaveKey, settingConfig.DefaultFloatValue); break;
                case SettingType.Enum: setting = new EnumSetting(_settingsSaveLoader, settingConfig.SaveKey, settingConfig.DefaultEnumValueIndex, settingConfig.AllowedValueIndexes); break;
                case SettingType.Int: setting = new IntSetting(_settingsSaveLoader, settingConfig.SaveKey, settingConfig.DefaultIntValue); break;
                case SettingType.Bool: setting = new BoolSetting(_settingsSaveLoader, settingConfig.SaveKey, settingConfig.DefaultBoolValue); break;
                default: throw new Exception($"Unknown setting type: {settingConfig.Type}");
            }

            setting.Load();

            return setting;
        }
    }
}