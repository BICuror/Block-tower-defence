using System;

namespace CuroSettings
{
    public abstract class SettingApplier<T> where T : Setting
    {
        public Setting _setting;
        
        protected static T FetchSetting(SettingsEnum key, Action applySetting)
        {
            if (SettingsContainer.SettingsExists(key))
            {
                T setting = SettingsContainer.GetSetting<T>(key);

                setting.ValueChanged += applySetting;

                return setting;
            }

            throw new Exception($"Setting applier could not find setting with key: {key}");
        }
    }
}