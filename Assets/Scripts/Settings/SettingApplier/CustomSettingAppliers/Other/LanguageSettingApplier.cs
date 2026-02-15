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
            
            if (localizationSettings.SupportedLanguages.Exists(languageData => languageData.Language == Application.systemLanguage)) 
            { 
                Setting.SetValue(Application.systemLanguage);
            }
            else Setting.SetValue(localizationSettings.DefaultLanguage);
        }
        
        private static void ApplyNewSettingValue()
        {
            LocalizationManager.SetLanguage(Setting.GetValue<SystemLanguage>());
        }
    }
}