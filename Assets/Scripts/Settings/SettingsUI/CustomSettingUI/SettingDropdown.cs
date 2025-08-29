using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    
    public abstract class SettingDropdown<T> : SettingUI<EnumSetting> where T : Enum
    {
        private TMP_Dropdown _dropdown;
        
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(SetSettingValue);
            
            InitializeDropdown();
            
            base.Awake();
        }

        private void InitializeDropdown()
        {
            int optionsAmount = Enum.GetValues(typeof(T)).Length;

            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            
            for (int i = 0; i < optionsAmount; i++)
            {
                string optionName = Enum.GetName(typeof(T), i);
                
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(optionName);
                
                optionDatas.Add(optionData);
            }

            _dropdown.options = optionDatas;
            _dropdown.RefreshShownValue();
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