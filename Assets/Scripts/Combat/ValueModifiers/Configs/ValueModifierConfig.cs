using System.Collections.Generic;
using UnityEngine;
using Configs;
using System;

namespace Combat.ValueModifiers
{
    [CreateAssetMenu(fileName = "ValueModifierConfig", menuName = "Combat/ValueModifiers/ValueModifierConfig")]

    public sealed class ValueModifierConfig : UpdatableScriptableObject
    {
        [SerializeField] private List<ValueModifierConfigEntryContainer> _valueModifierConfigEntries;
        
        public IReadOnlyList<ValueModifierConfigEntryContainer> ValueModifierConfigEntries => _valueModifierConfigEntries;
        
        protected override IEnumerable<INonScriptableObjectUpdatable> GetAllNonScriptableObjectUpdatables()
        {
            return _valueModifierConfigEntries;
        }
    }
    
    [Serializable] public sealed class ValueModifierConfigEntryContainer : ConfigEntry<ValueModifierTypeConfig> {}
}