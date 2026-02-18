using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(CustomDropdown))]
    
    public abstract class SettingCustomDropdown<T> : SettingUI<EnumSetting> where T : Enum
    {
        private CustomDropdown _dropdown;
        
        private void Awake()
        {
            _dropdown = GetComponent<CustomDropdown>();
            
            base.Awake();
            
            InitializeDropdown();
            _dropdown.SetSelectedItem(Setting.GetValueIndex());
            
            _dropdown.SelectedValueUpdated += SetSettingValue;
        }

        private void InitializeDropdown()
        {
            List<int> validOptions = Enum.GetValues(typeof(T)).Cast<int>().Intersect(Setting.AllowedValueIndexes).ToList();

            _dropdown.SetItemValues(validOptions);
        }

        private void SetSettingValue(int value)
        {
            Setting.SetValueIndex(value);
        }
        
        protected override void UpdateSettingState()
        {
            _dropdown.SetSelectedItem(Setting.GetValueIndex());
        }
    }
}