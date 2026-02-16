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
        string valueText = $"({Math.Round(_stat.Default, 2)} + {_stat.GetFlatModifier():F2}) * {Math.Round(_stat.GetMultiplierModifier() * 100):F0}% = {_stat.Value:F2}";
        
        _valueTextField.text = valueText;
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        _stat.ValueChanged -= UpdateStatValueDisplay;
    } 
}