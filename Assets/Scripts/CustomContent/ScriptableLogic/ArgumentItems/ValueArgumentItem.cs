using Combat.ValueModifiers;
using Combat.Stats;
using UnityEngine;
using Configs;

namespace ScriptableLogic.ArgumentItem
{
    public sealed class ValueArgumentItem : ArgumentItem
    {
        [Argument("DefaultStaticValue")] private float _defaultStaticValue;
        [Argument("UseStatValue")] private bool _useStatValue;
        [Argument("ScaleStatConfig")] private StatConfig _scaleStatConfig;

        public bool UseStatValue => _useStatValue;

        public Stat GetStat() => Args.Owner.StatContainer.Get(_scaleStatConfig.GetStatType());
        
        public ValueModificationContainer GetValueModificationContainer()
        {
            if (_useStatValue)
            {
                return Args.Owner.StatContainer.Get(_scaleStatConfig.GetStatType()).GetValueModificationContainer();
            }

            return new ValueModificationContainer(_defaultStaticValue);
        }

        public float GetValue()
        {
            if (_useStatValue) return Args.Owner.StatContainer.Get(_scaleStatConfig.GetStatType()).Value;
            
            return _defaultStaticValue;
        }

        public int GetRoundedValue()
        {
            if (_useStatValue) return Args.Owner.StatContainer.Get(_scaleStatConfig.GetStatType()).RoundedValue;
            
            return Mathf.RoundToInt(_defaultStaticValue);
        }
    }
}