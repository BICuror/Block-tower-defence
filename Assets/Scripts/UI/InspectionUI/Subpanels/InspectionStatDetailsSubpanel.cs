using UnityEngine;
using System;
using TMPro;

public sealed class InspectionStatDetailsSubpanel : InspectionSubpanelBase
{
    [SerializeField] private TextMeshProUGUI _valueTextField;

    private Stat _stat;
    
    public void Initialize(Stat stat, StatTooltipTagData tagData)
    {
        _stat = stat;

        SetTagData(tagData);
        
        stat.ValueChanged += _ => UpdateStatValueDisplays();
        UpdateStatValueDisplays();
    }

    private void UpdateStatValueDisplays()
    {
        string valueText = $"({Math.Round(_stat.Default, 2)} + {Math.Round(_stat.GetFlatModifier())}) * {Math.Round(_stat.GetMultiplierModifier() * 100):F2} = {_stat.Value:F2}";
        
        _valueTextField.text = valueText;
    }
    
    private void OnDestroy() => _stat.ValueChanged -= _ => UpdateStatValueDisplays();
}