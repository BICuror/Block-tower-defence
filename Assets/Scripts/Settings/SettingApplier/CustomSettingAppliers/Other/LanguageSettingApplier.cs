using CuroLocalization;
using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class LanguageSettingApplier : SettingApplier<EnumSetting>
    {
        private static EnumSetting _setting; 
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            TryGetSystemLanguage();
            
            _setting = FetchSetting(SettingsEnum.Language, ApplyNewSettingValue);
            ApplyNewSettingValue();
        }

        private static void TryGetSystemLanguage()
        {
            _setting = SettingsContainer.GetSetting<EnumSetting>(SettingsEnum.Language);
            
            if (_setting.GetValueIndex() != -1) return;

            LocalizationSettings localizationSettings = LocalizationManager.Settings;

            SystemLanguage resultSystemLanguage = localizationSettings.DefaultLanguage;
            
            if (localizationSettings.SupportedLanguages.Exists(languageData => languageData.Language == Application.systemLanguage)) 
            { 
                resultSystemLanguage = Application.systemLanguage;
            }
            
            _setting.SetValue(LocalizationManager.GetSupportedLanguageData(resultSystemLanguage).SerializableLanguage);
        }
        
        private static void ApplyNewSettingValue()
        {
            SystemLanguage language = LocalizationManager.GetSupportedLanguageData(_setting.GetValue<SerializableSystemLanguage>()).Language;

            if (!LocalizationManager.Settings.SupportedLanguages.Exists(supportedLanguageData => supportedLanguageData.Language == language))
            {
                Debug.Log($"{language} was tired to applied");
                language = SystemLanguage.English;
            }
            
            LocalizationManager.SetLanguage(language);
        }
    }
}