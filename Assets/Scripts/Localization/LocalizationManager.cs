using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;

namespace CuroLocalization
{
    public static class LocalizationManager
    {
        private const string LOCALIZATION_SETTINGS_PATH = "Localization/LocalizationSettings";

        private static Dictionary<string, string> _currentLanguageLocalization;
        private static LocalizationSettings _localizationSettings;
        private static SystemLanguage _currentLanguage;

        public static LocalizationSettings Settings
        {
            get
            {
                if (!_localizationSettings) FetchLocalizationSettings();
                
                return _localizationSettings;
            }
        }

        public static SystemLanguage CurrentLanguage => _currentLanguage;
        
        public static event Action OnLanguageChanged;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeLocalization()
        {
            FetchLocalizationSettings();
        }
        
        public static string GetLocalization(string key) => _currentLanguageLocalization[key];
        
        public static void SetLanguage(SystemLanguage language)
        {
            SupportedLanguageData supportedLanguageData = GetSupportedLanguageData(language);

            string filePath = Path.Combine(Settings.LocalizationFilesPath, supportedLanguageData.LanguageColumnKey);
            
            string file = Resources.Load<TextAsset>(filePath).text;
            
            _currentLanguageLocalization = JsonConvert.DeserializeObject<Dictionary<string, string>>(file);
            _currentLanguage = language;
            
            OnLanguageChanged?.Invoke();
        }

        public static SupportedLanguageData GetSupportedLanguageData(SystemLanguage language)
        {
            return Settings.SupportedLanguages.Find(data => data.Language == language);
        }

        public static SupportedLanguageData GetSupportedLanguageData(SerializableSystemLanguage _serializableSystemLanguage)
        {
            return Settings.SupportedLanguages.Find(data => data.SerializableLanguage == _serializableSystemLanguage);
        } 
        
        private static void FetchLocalizationSettings()
        {
            _localizationSettings = Resources.Load<LocalizationSettings>(LOCALIZATION_SETTINGS_PATH);
        }
    }
    
    public enum SerializableSystemLanguage
    { 
        English = 0, 
        Russian = 1,
    }
}