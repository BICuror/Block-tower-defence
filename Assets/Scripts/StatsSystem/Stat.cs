using System.Collections.Generic;
using UnityEngine;
using System;

public class Stat
{
    private List<StatModifier> _statModifiers = new(0);
    private float _default;
    private float _flat;
    private float _multiplier = 1f;
    private float _value;
    private int _roundedValue;

    protected virtual float MinimalValue { get => float.MinValue; }
    
    public float Default => _default;
    public float Value => _value;
    public int RoundedValue => _roundedValue;

    public Action<float> ValueChanged;
    public Action<int> RoundedValueChanged;

    public void Reset()
    {
        _flat = 0f;
        _multiplier = 1f;
        
        _statModifiers.ForEach(RemoveStatModifier);
    }
    
    public void SetDefault(float value)
    {
        _default = value;
        CalculateStatValue();
    }
    public void ChangeFlat(float value)
    {
        _flat += value;
        CalculateStatValue();
    }
    public void ChangeMultiplier(float value)
    {
        _multiplier += value;
        CalculateStatValue();
    }

    public void AddStatModifier(StatModifier statModifier)
    {
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

    public float GetFlatModifier()
    {
        float flat = _flat;
        
        _statModifiers.ForEach(modifier => flat += modifier.Flat);

        return flat;
    }
    
    public float GetMultiplierModifier()
    {
        float multiplier = _multiplier;
        
        _statModifiers.ForEach(modifier => multiplier += modifier.Multiplier);

        return multiplier;
    }
    
    private void CalculateStatValue()
    {
        float flatAddition = _flat;
        float multiplier = _multiplier;
        
        for (int i = 0; i < _statModifiers.Count; i++)
        {
            flatAddition += _statModifiers[i].Flat;
            multiplier += _statModifiers[i].Multiplier;
        }
        
        _value = (_default + flatAddition) * multiplier;

        if (_value < MinimalValue) _value = MinimalValue;
        
        _roundedValue = Mathf.RoundToInt(_value);

        ValueChanged?.Invoke(_value);
        RoundedValueChanged?.Invoke(_roundedValue);
    }
}