using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace CuroSettings
{
    [CreateAssetMenu(fileName = "SettingsConfig", menuName = "Settings/SettingsConfig")]
    
    public sealed class SettingsConfig : ScriptableObject
    {
        [SerializeField] private List<SettingConfig> _settingConfigs;

        public List<SettingConfig> SettingConfigs => new(_settingConfigs);
        
#if UNITY_EDITOR
        public bool CheckForDuplicateSaveKeys()
        {
            bool containsDuplicateIDs = false;
            
            for (int outerScope = 0; outerScope < _settingConfigs.Count; outerScope++)
            {
                string outerScopeID = _settingConfigs[outerScope].SaveKey;
                
                for (int innerScope = outerScope + 1; innerScope < _settingConfigs.Count; innerScope++)
                {
                    string innerScopeID = _settingConfigs[innerScope].SaveKey;
                    
                    if (outerScopeID == innerScopeID)
                    {
                        Debug.LogError($"Duplicate SettingConfig id: {_settingConfigs[outerScope].SaveKey} on indexes {outerScope} and {innerScope}");
                        containsDuplicateIDs = true;
                    }
                }
            }

            return containsDuplicateIDs;
        }
        
        public void ApplyEnumValues()
        {
            for (int i = 0; i < _settingConfigs.Count; i++)
            {
                if (Enum.TryParse(typeof(SettingsEnum), _settingConfigs[i].SaveKey, true, out object enumValue))
                {
                    _settingConfigs[i].SetSettingType((SettingsEnum)enumValue);
                }
                else
                {
                    Debug.LogError($"Could not parse {_settingConfigs[i].SaveKey} to AudioEnum, try regenerating AudioEnum");
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}