using UnityEngine;

namespace CuroSettings
{
    public sealed class PlayerPrefsSettingsSaveLoader : ISettingsSaveLoader
    {
        void ISettingsSaveLoader.ResetSettings()
        {
            PlayerPrefs.DeleteAll();
        }

        #region Float

        float ISettingsSaveLoader.GetFloatSetting(string key, float defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key);
            }
            
            PlayerPrefs.SetFloat(key, defaultValue);
            
            return defaultValue;
        }

        void ISettingsSaveLoader.SaveFloatSetting(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        #endregion

        #region Int
        
        int ISettingsSaveLoader.GetIntSetting(string key, int defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }
            
            PlayerPrefs.SetInt(key, defaultValue);
            
            return defaultValue;
        }

        void ISettingsSaveLoader.SaveIntSetting(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        #endregion
    }
}