using System.Collections.Generic;
using UnityEngine;
using System;

namespace CuroSettings
{
    public sealed class EnumSetting : Setting
    {
        private readonly int _defaultValueIndex;
        private readonly List<int> _allowedValueIndexes;
        private int _valueIndex;
        
        public List<int> AllowedValueIndexes => _allowedValueIndexes;
        
        public EnumSetting(ISettingsSaveLoader saveLoader, string key, int defaultValueIndex, List<int> allowedValueIndexes) : base(saveLoader, key)
        {
            _defaultValueIndex = defaultValueIndex;
            _allowedValueIndexes = allowedValueIndexes;
        }

        public T GetValue<T>() where T : Enum
        {
            return (T)(object)_valueIndex;
        }

        public void SetValue<T>(T value) where T : Enum
        {
            SetValueIndex((int)(object)value);
        }

        public void SetValueIndex(int value)
        {
            if (!_allowedValueIndexes.Contains(value)) Debug.Log($"Tried to set enum {value} to setting {Key}, which is not in allowedValueIndexesList in SettingsConfig");
            
            _valueIndex = value;
            
            ValueChanged?.Invoke();
        }

        public int GetValueIndex()
        {
            return _valueIndex;
        }
        
        public override void Load()
        {
            SetValueIndex(SaveLoader.GetIntSetting(Key, _defaultValueIndex));
                
            ValueLoaded?.Invoke();
        }

        public override void Save()
        {
            SaveLoader.SaveIntSetting(Key, _valueIndex);
        }
    }
}