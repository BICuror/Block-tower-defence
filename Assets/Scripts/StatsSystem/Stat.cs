using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;

public class Stat
{
    private List<StatModifier> _statModifiers = new(0);
    private float _default;
    private float _flat;
    private float _multiplier = 1f;
    private float _value;
    private int _roundedValue;

    protected virtual float MinimalValue => 0; 
    //used purely for ui
    public virtual bool LowValueIsGood => false;
    
    public float Default => _default;
    public float Value => _value;
    public int RoundedValue => _roundedValue;
    public bool IsModified => _statModifiers.Count > 0 || _flat != 0 || _multiplier != 1f;

    public Action<float> ValueChanged;
    public Action<int> RoundedValueChanged;

    public void Reset()
    {
        _flat = 0f;
        _multiplier = 1f;

        while (_statModifiers.Count > 0)
        {
            RemoveStatModifier(_statModifiers[0]);
        }
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

    public bool IsApplied(StatModifier statModifier) => _statModifiers.Contains(statModifier);
    
    public void AddStatModifier(StatModifier statModifier)
    {
        if (IsApplied(statModifier)) throw new DuplicateNameException("Tried to add same stat modifier twice");
        
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