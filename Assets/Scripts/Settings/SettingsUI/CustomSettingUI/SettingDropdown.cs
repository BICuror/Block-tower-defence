using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

namespace CuroSettings.UI
{
    [RequireComponent(typeof(TMP_Dropdown))]
    
    public abstract class SettingDropdown<T> : SettingUI<EnumSetting> where T : Enum
    {
        private readonly Dictionary<int, int> _languageIndexes = new();
        private TMP_Dropdown _dropdown;
        
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(SetSettingValue);
            
            base.Awake();
            
            InitializeDropdown();
        }

        private void InitializeDropdown()
        {
            _languageIndexes.Clear();
            
            List<int> validOptions = Enum.GetValues(typeof(T)).Cast<int>().Intersect(Setting.AllowedValueIndexes).ToList();

            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            
            for (int i = 0; i < validOptions.Count; i++)
            {
                string optionName = Enum.GetName(typeof(T), validOptions[i]);
                
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(optionName);

                optionDatas.Add(optionData);
                _languageIndexes.Add(i, validOptions[i]);
            }

            _dropdown.options = optionDatas;
            _dropdown.value = Setting.GetValueIndex();
            _dropdown.RefreshShownValue();
        }

        private void SetSettingValue(int value)
        {
            Setting.SetValueIndex(_languageIndexes[value]);
        }
        
        protected override void UpdateSettingState()
        {
            _dropdown.value = Setting.GetValueIndex();
        }
    }
}