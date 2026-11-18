using System.Collections.Generic;
using Combat.ValueModifiers;
using UnityEngine;
using System.Data;
using System;

namespace Combat.Stats
{
    public abstract class Stat
    {
        private List<StatModifier> _statModifiers = new(0);
        private float _defaultValue;
        private int _roundedValue;
        private float _value;
    
        private float _flatModifier;
        private float _multModifier = 1f;
        
        protected virtual float MinimalValue => 0;
        
        public float DefaultValue => _defaultValue;
        public float Value => _value;
        public int RoundedValue => _roundedValue;
        public float FlatModifier => _flatModifier;
        public float MultModifier => _multModifier;
    
        public event Action<float> ValueChanged;
        public event Action<int> RoundedValueChanged;
    
        public void Reset()
        {
            foreach (StatModifier statModifier in _statModifiers)
            {
                statModifier.ModifierChanged -= CalculateStatValue;
            }
            
            _statModifiers.Clear();
            CalculateStatValue();
        }
        
        public void SetDefault(float value)
        {
            _defaultValue = value;
            CalculateStatValue();
        }
    
        public void AddStatModifier(StatModifier statModifier)
        {
            if (_statModifiers.Contains(statModifier)) throw new DuplicateNameException("Tried to add same stat modifier twice");
            
            statModifier.ModifierChanged += CalculateStatValue;
            _statModifiers.Add(statModifier);
            CalculateStatValue();
        }
    
        public void RemoveStatModifier(StatModifier statModifier)
        {
            statModifier.ModifierChanged -= CalculateStatValue;
            _statModifiers.Remove(statModifier);
            CalculateStatValue();
        }
    
        public ValueModificationContainer GetValueModificationContainer()
        {
            ValueModificationContainer valueModificationContainer = new ValueModificationContainer()
            {
                InitialValue = _defaultValue,
                FlatChange = _flatModifier,
                MultChange = _multModifier
            };
    
            return valueModificationContainer;
        }
        
        private void CalculateStatValue()
        {
            float flatAddition = 0;
            float multAddition = 1f;
            
            for (int i = 0; i < _statModifiers.Count; i++)
            {
                flatAddition += _statModifiers[i].Flat;
                multAddition += _statModifiers[i].Multiplier;
            }
    
            _flatModifier = flatAddition;
            _multModifier = multAddition;
            
            _value = (_defaultValue + flatAddition) * multAddition;
    
            if (_value < MinimalValue) _value = MinimalValue;
            
            _roundedValue = Mathf.RoundToInt(_value);
    
            ValueChanged?.Invoke(_value);
            RoundedValueChanged?.Invoke(_roundedValue);
        }
    }
}