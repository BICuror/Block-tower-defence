using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(Dropdown))]
    
    public abstract class SettingDropdown<T> : SettingUI<EnumSetting> where T : Enum
    {
        private Dropdown _dropdown;
        
        private void Awake()
        {
            _dropdown = GetComponent<Dropdown>();
            _dropdown.onValueChanged.AddListener(SetSettingValue);
            
            InitializeDropdown();
            
            base.Awake();
        }

        private void InitializeDropdown()
        {
            int optionsAmount = Enum.GetValues(typeof(T)).Length;

            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            
            for (int i = 0; i < optionsAmount; i++)
            {
                string optionName = Enum.GetName(typeof(T), i);
                
                Dropdown.OptionData optionData = new Dropdown.OptionData(optionName);
                
                optionDatas.Add(optionData);
            }
        }

        private void SetSettingValue(int value)
        {
            Setting.SetValueIndex(value);
        }
        
        protected override void OnSettingValueLoaded()
        {
            _dropdown.value = Setting.GetValueIndex();
        }
    }
}