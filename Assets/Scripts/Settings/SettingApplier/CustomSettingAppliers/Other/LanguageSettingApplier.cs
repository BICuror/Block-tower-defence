using CuroLocalization;
using UnityEngine;

namespace CuroSettings.CustomSettingAppliers
{
    public sealed class LanguageSettingApplier : SettingApplier<EnumSetting>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            TryGetSystemLanguage();
            
            FetchSetting(SettingsEnum.Language, ApplyNewSettingValue);
        }

        private static void TryGetSystemLanguage()
        {
            Setting = SettingsContainer.GetSetting<EnumSetting>(SettingsEnum.Language);
            
            if (Setting.GetValueIndex() != -1) return;

            LocalizationSettings localizationSettings = LocalizationManager.Settings;

            SystemLanguage resultSystemLanguage = localizationSettings.DefaultLanguage;
            
            if (localizationSettings.SupportedLanguages.Exists(languageData => languageData.Language == Application.systemLanguage)) 
            { 
                resultSystemLanguage = Application.systemLanguage;
            }
            
            Setting.SetValue(LocalizationManager.GetSupportedLanguageData(resultSystemLanguage).SerializableLanguage);
        }
        
        private static void ApplyNewSettingValue()
        {
            SystemLanguage language = LocalizationManager.GetSupportedLanguageData(Setting.GetValue<SerializableSystemLanguage>()).Language;

            if (!LocalizationManager.Settings.SupportedLanguages.Exists(supportedLanguageData => supportedLanguageData.Language == language))
            {
                Debug.Log($"{language} was tired to applied");
                language = SystemLanguage.English;
            }
            
            LocalizationManager.SetLanguage(language);
        }
    }
}