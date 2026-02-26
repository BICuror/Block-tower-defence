#if UNITY_EDITOR

using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace CuroLocalization
{
    public sealed class ImportLocalizationFromTsv
    {
        [MenuItem("Tools/Localization/ImportFromCSV")]
        public static void ImportFromCSV()
        {
            if (!TSVImport.CreateTableFromCSV(out var table))
            {
                Debug.LogError("File was not selected.");
                return;
            }

            List<LanguageData> languages = ParseLanguages(table);

            SaveLanguages(languages, LocalizationManager.Settings.LocalizationFilesPath);

            Debug.Log("Successfully imported localization files");
            AssetDatabase.Refresh();
        }

        private static List<LanguageData> ParseLanguages(Table table)
        {
            List<LanguageData> tableLanguages = new List<LanguageData>();

            for (int i = 1; i < table.GetLength(0); i++)
            {
                if (IsSupportedLanguage(table.ByTableIndex[i, 0]))
                {
                    tableLanguages.Add(new LanguageData(table.ByTableIndex[i, 0], i));
                }
            }

            for (int i = 0; i < tableLanguages.Count; i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    string key = table.ByTableIndex[0, j];
                    string value = table.ByTableIndex[tableLanguages[i].Column, j];

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        CheckDictionary(tableLanguages[i], key, value);
                    }
                }
            }

            return tableLanguages;
        }
        
        private static bool IsSupportedLanguage(string key)
        {
            return !string.IsNullOrEmpty(key) && LocalizationManager.Settings.SupportedLanguages.Exists(language => language.LanguageColumnKey == key);
        }
        
        private static void CheckDictionary(LanguageData languageData, string key, string value)
        {
            if (!languageData.Content.TryAdd(key, value))
            {
                Debug.LogError($"Multiple keys: \"{key}\"");
            }
        }

        private static void SaveLanguages(List<LanguageData> languages, string directory)
        {
            foreach (var lang in languages)
            {
                if (!IsSupportedLanguage(lang.Name)) continue;

                string savePath = Path.Combine(directory, lang.Name + ".json");
                
                if (File.Exists(savePath))
                {
                    var data = File.ReadAllText(savePath);
                    var existingDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                    foreach (var pair in lang.Content)
                    {
                        existingDictionary[pair.Key] = pair.Value;
                    }

                    File.WriteAllText(savePath, JsonConvert.SerializeObject(existingDictionary, Formatting.Indented));
                }
                else
                {
                    File.WriteAllText(savePath, JsonConvert.SerializeObject(lang.Content, Formatting.Indented));
                }
            }
        }
    }
}

#endif