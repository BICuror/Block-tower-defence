using UnityEngine;

namespace CuroSettings.UI
{
    public abstract class SettingUI<T> : MonoBehaviour where T : Setting
    {
        [SerializeField] private SettingsEnum _settingKey;
        protected T Setting;
        
        protected void Awake()
        {
            Setting = SettingsContainer.Instance.GetSetting<T>(_settingKey);
            
            Setting.ValueLoaded += OnSettingValueLoaded;
            
            OnSettingValueLoaded();
        }

        protected void OnDestroy()
        {
            Setting.ValueLoaded -= OnSettingValueLoaded;
        }
        
        protected abstract void OnSettingValueLoaded();
    }
}