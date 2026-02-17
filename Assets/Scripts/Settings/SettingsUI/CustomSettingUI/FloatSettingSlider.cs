using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(Slider))]
    
    public sealed class FloatSettingSlider : SettingUI<FloatSetting>
    {
        [SerializeField] private TextMeshProUGUI _settingValueText;
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
        
        protected override void UpdateSettingState()
        {
            _slider.value = Setting.Value;
            _settingValueText.text = $"{Setting.Value:F2}";
        }
    }
}