using System;

namespace CuroSettings
{
    public interface ISettingsSaveLoader
    {
        public void ResetSettings();
        public float GetFloatSetting(string key, float defaultValue);
        public void SaveFloatSetting(string key, float value);
        
        public int GetIntSetting(string key, int defaultValue);
        public void SaveIntSetting(string key, int value);
    }
}