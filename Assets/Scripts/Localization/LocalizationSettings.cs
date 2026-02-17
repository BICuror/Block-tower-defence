using System.Collections.Generic;
using UnityEngine;
using System;

namespace CuroLocalization
{
    [CreateAssetMenu(fileName = "LocalizationSettings", menuName = "CuroLocalization/Localization Settings")]
    
    public sealed class LocalizationSettings : ScriptableObject
    {
        [Space] [Header("FilesSettings")]
        [SerializeField] private string _localizationFilesPath = "Assets/Resources/Localization/Files";
        
        [Space] [Header("TableParsing")]
        [SerializeField] private string _tableCellSeparatorSymbol = "#TABLE_CELL_SEPARATOR#";
        [SerializeField] private string _tableLingSeparatorSymbol = "#TABLE_ROW_SEPARATOR#";
        
        [Space] [Header("SupportedLanguages")]
        [SerializeField] private List<SupportedLanguageData> _supportedLanguages;
        [SerializeField] private SystemLanguage _defaultLanguage = SystemLanguage.English;
        
        public string LocalizationFilesPath => _localizationFilesPath;
        public string TableCellSeparatorSymbol => _tableCellSeparatorSymbol;
        public string TableLingSeparatorSymbol => _tableLingSeparatorSymbol;
        public List<SupportedLanguageData> SupportedLanguages => _supportedLanguages;
        public SystemLanguage DefaultLanguage => _defaultLanguage;
    }
    
    [Serializable] public sealed class SupportedLanguageData
    {
        [SerializeField] private string _languageColumnKey = "ENGLISH";
        [SerializeField] private SystemLanguage _systemLanguage = SystemLanguage.English;
        [SerializeField] private SerializableSystemLanguage _serializableLanguage = SerializableSystemLanguage.English;
        
        public string LanguageColumnKey => _languageColumnKey;
        public SystemLanguage Language => _systemLanguage;
        public SerializableSystemLanguage SerializableLanguage => _serializableLanguage;
    }
}