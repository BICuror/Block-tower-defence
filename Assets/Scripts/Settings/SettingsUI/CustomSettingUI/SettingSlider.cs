using UnityEngine.UI;
using UnityEngine;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(Slider))]
    
    public sealed class SettingSlider : SettingUI<FloatSetting>
    {
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(SetSettingValue);
            
            base.Awake();
        }

        private void SetSettingValue(float value)
        {
            Setting.SetValue(value);    
        }
        
        protected override void OnSettingValueLoaded()
        {
            _slider.value = Setting.Value;
        }
    }
}