using System.Collections.Generic;
using UnityEngine;
using System;

public class Stat
{
    private List<StatModifier> _statModifiers = new(0);
    private float _defaultValue;
    private float _flatAddition = 0f;
    private float _totalMultiplier = 1f;
    private float _value;
    private int _roundedValue;

    public float Value => _value;
    public int RoundedValue => _roundedValue;
    public float TotalMultiplier => _totalMultiplier;
    public float FlatAddition => _flatAddition;
    public float DefaultValue => _defaultValue;

    public Action<float> ValueChanged;
    public Action<int> RoundedValueChanged;

    public void Reset()
    {
        _flatAddition = 0f;
        _totalMultiplier = 1f;
    }
    public void SetDefault(float value)
    {
        _defaultValue = value;
        CalculateStatValue();
    }
    public void ChangeFlat(float value)
    {
        _flatAddition += value;
        CalculateStatValue();
    }
    public void ChangeMultiplier(float value)
    {
        _totalMultiplier += value;
        CalculateStatValue();
    }

    public void AddStatModifier(StatModifier statModifier)
    {
        _statModifiers.Add(statModifier);
        CalculateStatValue();
    }

    public void RemoveStatModifier(StatModifier statModifier)
    {
        _statModifiers.Remove(statModifier);
        CalculateStatValue();
    }
    
    private void CalculateStatValue()
    {
        float flatAddition = _flatAddition;
        float multiplier = _totalMultiplier;
        
        for (int i = 0; i < _statModifiers.Count; i++)
        {
            flatAddition += _statModifiers[i].FlatModifier;
            multiplier += _statModifiers[i].TotalMultiplier;
        }
        
        _value = (_defaultValue + flatAddition) * multiplier;
        _roundedValue = Mathf.RoundToInt(_value);

        ValueChanged?.Invoke(_value);
        RoundedValueChanged?.Invoke(_roundedValue);
    }
}
