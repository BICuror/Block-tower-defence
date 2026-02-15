using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class UIScaleSettingApplier : MonoBehaviour
    {
        private FloatSetting _scaleSetting;
        private Vector2 _defaultScale;
        
        private void Awake()
        {
            _defaultScale = transform.localScale;
            
            _scaleSetting = SettingsContainer.GetSetting<FloatSetting>(SettingsEnum.UIScale);
            _scaleSetting.ValueChanged += UpdateSetting;
            UpdateSetting();
        }

        private void UpdateSetting()
        {
            transform.localScale = _defaultScale * _scaleSetting.Value;
        }
        
        private void OnDestroy()
        {
            _scaleSetting.ValueChanged -= UpdateSetting;
        }
    }
}