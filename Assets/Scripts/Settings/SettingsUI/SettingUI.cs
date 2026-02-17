using UnityEngine;

namespace CuroSettings.UI
{
    public abstract class SettingUI<T> : MonoBehaviour where T : Setting
    {
        [SerializeField] private SettingsEnum _settingKey;
        protected T Setting;
        
        protected void Awake()
        {
            Setting = SettingsContainer.GetSetting<T>(_settingKey);
            
            Setting.ValueLoaded += UpdateSettingState;
            Setting.ValueChanged += UpdateSettingState;
            
            UpdateSettingState();
        }

        protected void OnDestroy()
        {
            Setting.ValueLoaded -= UpdateSettingState;
            Setting.ValueChanged -= UpdateSettingState;
        }
        
        protected abstract void UpdateSettingState();
    }
}