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
            SupportedLanguageData supportedLanguageData = Settings.SupportedLanguages.Find(data => data.Language == language);
            
            string filePath = Path.Combine(Settings.LocalizationFilesPath, supportedLanguageData.LanguageColumnKey + ".json");
            
            _currentLanguageLocalization = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));
            _currentLanguage = language;
            
            OnLanguageChanged?.Invoke();
        }
        
        private static void FetchLocalizationSettings()
        {
            _localizationSettings = Resources.Load<LocalizationSettings>(LOCALIZATION_SETTINGS_PATH);
        }
    }
}