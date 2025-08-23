namespace CuroSettings
{
    public sealed class IntSetting : Setting
    {
        private readonly int _defaultValue;
        private int _value;
        
        public int Value => _value;

        public IntSetting(ISettingsSaveLoader saveLoader, string key, int defaultValue) : base(saveLoader, key)
        {
            _defaultValue = defaultValue;
        }
        
        public void SetValue(int value)
        {
            _value = value;
            
            ValueChanged?.Invoke();
        }

        public override void Load()
        {
            SetValue(SaveLoader.GetIntSetting(Key, _defaultValue));
            
            ValueLoaded?.Invoke();
        }

        public override void Save()
        {
            SaveLoader.SaveIntSetting(Key, _value);
        }
    }
}