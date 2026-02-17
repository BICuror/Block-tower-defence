using NaughtyAttributes;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CuroSettings
{
    [Serializable] public sealed class SettingConfig
    {
        [Header("SaveKey")] 
        [SerializeField] private string _saveKey;
        [SerializeField] private SettingsEnum _settingEnum;
        
        [Header("Value")]
        [SerializeField] private SettingType _type;

        [AllowNesting] [ShowIf("_type", SettingType.Float)] [SerializeField] private float _defaultFloatValue;
        [AllowNesting] [ShowIf("_type", SettingType.Enum)] [SerializeField] private int _defaultEnumValueIndex;
        [AllowNesting] [ShowIf("_type", SettingType.Int)] [SerializeField] private int _defaultIntValue;
        [AllowNesting] [ShowIf("_type", SettingType.Bool)] [SerializeField] private bool _defaultBoolValue;
        [AllowNesting] [ShowIf("_type", SettingType.Enum)] [SerializeField] private AllowedValueIndexesContainer _allowedValueIndexesContainer;

        public string SaveKey => _saveKey;
        public SettingsEnum SettingEnum => _settingEnum;
        public SettingType Type => _type;

        public float DefaultFloatValue => _defaultFloatValue;
        public int DefaultEnumValueIndex => _defaultEnumValueIndex;
        public int DefaultIntValue => _defaultIntValue;
        public bool DefaultBoolValue => _defaultBoolValue;
        public List<int> AllowedValueIndexes => _allowedValueIndexesContainer.Indexes;
        
        public void SetSettingType(SettingsEnum settingEnum) => _settingEnum = settingEnum;
    }

    [Serializable] public sealed class AllowedValueIndexesContainer
    {
        public List<int> Indexes;
    }
}