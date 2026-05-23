using UnityEngine;
using System;
using TMPro;

public sealed class InspectionStatDetailsTooltip : TooltipPanelBase
{
    [SerializeField] private TextMeshProUGUI _valueTextField;

    private Stat _stat;
    
    public void Initialize(Stat stat, StatTooltipTagData tagData)
    {
        _stat = stat;

        SetTagData(tagData);

        stat.ValueChanged += UpdateStatValueDisplay;
        UpdateStatValueDisplay();
    }

    private void UpdateStatValueDisplay(float _) => UpdateStatValueDisplay();
    private void UpdateStatValueDisplay()
    {
        string baseValueString = Math.Round(_stat.Default, 2).ToString();
        
        string flatModifierStringValue = "";

        if (_stat.GetFlatModifier() > 0) flatModifierStringValue = $" + {_stat.GetFlatModifier():F2}";
        else if (_stat.GetFlatModifier() < 0) flatModifierStringValue = $" - {_stat.GetFlatModifier():F2}";
        
        string multiplierModifierStringValue = $" * {Math.Round(_stat.GetMultiplierModifier() * 100):F0}%";

        string valueText;

        if (string.IsNullOrEmpty(flatModifierStringValue))
        {
            valueText = $"{baseValueString}{multiplierModifierStringValue} = {_stat.Value:F2}";
        }
        else
        {
            valueText = $"({baseValueString}{flatModifierStringValue}){multiplierModifierStringValue} = {_stat.Value:F2}";
        }
        
        _valueTextField.text = valueText;
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        _stat.ValueChanged -= UpdateStatValueDisplay;
    } 
}