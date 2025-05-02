using UnityEngine;
using System;
using TMPro;

public sealed class InspectionStatDetailsSubpanel : InspectionSubpanelBase
{
    [SerializeField] private TextMeshProUGUI _totalStatValueTextField;
    [SerializeField] private TextMeshProUGUI _multiplierStatValueTextField;
    [SerializeField] private TextMeshProUGUI _flatAdditionStatValueTextField;
    [SerializeField] private TextMeshProUGUI _baseStatValueTextField;

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
        _totalStatValueTextField.text = Math.Round(_stat.Value, 2).ToString();
        _multiplierStatValueTextField.text = $"{Math.Round(_stat.Multiplier * 100)}%"; 
        _flatAdditionStatValueTextField.text = Math.Round(_stat.Flat).ToString();
        _baseStatValueTextField.text = Math.Round(_stat.Default).ToString();
    }
    
    private void OnDestroy() => _stat.ValueChanged -= _ => UpdateStatValueDisplays();
}