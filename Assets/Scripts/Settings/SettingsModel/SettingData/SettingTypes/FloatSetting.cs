namespace CuroSettings
{
    public sealed class FloatSetting : Setting
    {
        private readonly float _defaultValue;
        private float _value;

        public float Value => _value;
        
        public FloatSetting(ISettingsSaveLoader saveLoader, string key, float defaultValue) : base(saveLoader, key)
        {
            _defaultValue = defaultValue;
        }
        
        public void SetValue(float value)
        {
            _value = value;
            
            ValueChanged?.Invoke();
        }

        public override void Load()
        {
            SetValue(SaveLoader.GetFloatSetting(Key, _defaultValue));
            
            ValueLoaded?.Invoke();
        }

        public override void Save()
        {
            SaveLoader.SaveFloatSetting(Key, _value);
        }
    }
}