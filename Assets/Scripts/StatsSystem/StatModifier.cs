using System;

public sealed class StatModifier
{
    private float _flat;
    private float _multiplier;
    
    public float Flat => _flat;
    public float Multiplier => _multiplier;
    
    public event Action ModifierChanged;

    public StatModifier(float flat = 0, float multiplier = 0)
    {
        _multiplier = multiplier;
        _flat = flat;
    }

    public void SetMultiplier(float value)
    {
        _multiplier = value;
        
        ModifierChanged?.Invoke();
    }

    public void SetFlat(float value)
    {
        _flat = value;
        
        ModifierChanged?.Invoke();
    }
}