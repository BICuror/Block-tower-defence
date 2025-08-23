using System;

namespace CuroSettings
{
    public sealed class EnumSetting : Setting
    {
        private readonly int _defaultValueIndex;
        private int _valueIndex;
        
        public EnumSetting(ISettingsSaveLoader saveLoader, string key, int defaultValueIndex) : base(saveLoader, key)
        {
            _defaultValueIndex = defaultValueIndex;
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