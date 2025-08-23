namespace CuroSettings
{
    public sealed class BoolSetting : Setting
    {
        private readonly bool _defaultValue;
        private bool _value;
        
        public bool Value => _value;

        public BoolSetting(ISettingsSaveLoader saveLoader, string key, bool defaultValue) : base(saveLoader, key)
        {
            _defaultValue = defaultValue;
        }

        public void SetValue(bool value)
        {
            _value = value;
            
            ValueChanged?.Invoke();
        }

        public override void Load()
        {
            int savedValue = SaveLoader.GetIntSetting(Key, ConvertBoolToInt(_defaultValue));

            SetValue(savedValue == 1);
            
            ValueLoaded?.Invoke();
        }

        public override void Save()
        {
            SaveLoader.SaveIntSetting(Key, ConvertBoolToInt(_value));
        }

        private int ConvertBoolToInt(bool value)
        {
            if (value) return 1;
            return 0;
        }
    }
}