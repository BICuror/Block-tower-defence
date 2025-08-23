using UnityEngine;
using System;

namespace CuroSettings
{
    [Serializable] public sealed class SettingConfig
    {
        [Header("RequiredData")] [SerializeField]
        private string _saveKey;

        [SerializeField] private SettingType _type;

        [Space] [Header("DefaultValues")] 
        [SerializeField] private float _defaultFloatValue;
        [SerializeField] private int _defaultEnumValueIndex;
        [SerializeField] private int _defaultIntValue;
        [SerializeField] private bool _defaultBoolValue;

        public string SaveKey => _saveKey;
        public SettingType Type => _type;

        public float DefaultFloatValue => _defaultFloatValue;
        public int DefaultEnumValueIndex => _defaultEnumValueIndex;
        public int DefaultIntValue => _defaultIntValue;
        public bool DefaultBoolValue => _defaultBoolValue;
    }
}