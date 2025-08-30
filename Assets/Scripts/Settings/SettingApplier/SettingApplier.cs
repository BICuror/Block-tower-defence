using UnityEngine;
using System;

namespace CuroSettings
{
    public abstract class SettingApplier<T> where T : Setting
    {
        private const bool THROW_EXCEPTION_WHEN_SETTING_IS_NOT_FOUND = false;
        
        protected static T Setting;
        
        protected static void FetchSetting(SettingsEnum key, Action applySetting)
        {
            if (SettingsContainer.Instance.SettingsExists(key))
            {
                Setting = SettingsContainer.Instance.GetSetting<T>(key);
                
                Setting.ValueChanged += applySetting;
                applySetting.Invoke();
            }
            else
            {
                if (THROW_EXCEPTION_WHEN_SETTING_IS_NOT_FOUND) throw new Exception($"Setting applier could not find setting with key: {key}");
                
                Debug.LogWarning($"Setting applier could not find setting with key: {key}");
            }
        }
    }
}