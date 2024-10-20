using System;
using UnityEngine;

public class Stat
{
    private float _defaultValue;
    private float _flatAddition = 0f;
    private float _totalMultiplier = 1f;
    private float _value;
    private int _roundedValue;

    public float Value => _value;
    public int RoundedValue => _roundedValue;

    public Action<float> ValueChanged;
    public Action<int> RoundedValueChanged;

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
    
    private void CalculateStatValue()
    {
        _value = (_defaultValue + _flatAddition) * _totalMultiplier;
        _roundedValue = Mathf.RoundToInt(_value);

        ValueChanged?.Invoke(_value);
        RoundedValueChanged?.Invoke(_roundedValue);
    }
}
