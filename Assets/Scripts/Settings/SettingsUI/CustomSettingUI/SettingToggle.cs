using UnityEngine.UI;
using UnityEngine;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(Toggle))]

    public sealed class SettingToggle : SettingUI<BoolSetting>
    {
        private Toggle _toggle;
        
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(SetSettingValue);
            
            base.Awake();
        }

        private void SetSettingValue(bool value)
        {
            Setting.SetValue(value);
        }
        
        protected override void OnSettingValueLoaded()
        {
            _toggle.isOn = Setting.Value;
        }
    }
}