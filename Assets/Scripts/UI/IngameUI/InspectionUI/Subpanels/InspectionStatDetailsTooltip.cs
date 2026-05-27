using CuroLocalization;
using UnityEngine;
using System;
using Combat;
using TMPro;

public sealed class InspectionStatDetailsTooltip : TooltipPanelBase
{
    private const string DEPENDENCY_STAT_VALUE_DISPLAY = "DependencyStatValue MainStatValue";

    [SerializeField] private Color _calculationColor = Color.grey;
    
    [Header("MainStat")]
    [SerializeField] private string _mainStatLabelLocKey;
    [SerializeField] private TextMeshProUGUI _mainStatValueTextField;

    [Header("DependencyStat")] 
    [SerializeField] private string _dependencyStatLabelLocKey;
    [SerializeField] private TextMeshProUGUI _dependencyStatValueTextField;
    private StatTooltipTagData _tagData;
    private Stat _dependencyStat;
    private Stat _mainStat;
    
    public void Initialize(CombatEntity combatEntity, StatTooltipTagData tagData)
    {
        _tagData = tagData;
        
        _mainStat = combatEntity.StatContainer.Get(Type.GetType(_tagData.AssociatedStatTypeName));
        _mainStat.ValueChanged += UpdateDisplayValues;

        if (_tagData.DependsOnStat)
        {
            _dependencyStat = combatEntity.StatContainer.Get(Type.GetType(_tagData.DependencyStatData.AssociatedStatTypeName));
            _dependencyStat.ValueChanged += UpdateDisplayValues;
        }

        SetTagData(_tagData);

        _dependencyStatValueTextField.gameObject.SetActive(_tagData.DependsOnStat);
        
        UpdateDisplayValues(0f);
    }

    private void UpdateDisplayValues(float _)
    {
        UpdateMainStatValueDisplay();
        
        if (_tagData.DependsOnStat) UpdateDependencyStatValueDisplay();
    }
    
    private void UpdateMainStatValueDisplay()
    {
        bool presentEverythingAsPercent = _tagData.PresentAsMultiplier;
        
        string result = GetParsedValue(_mainStat.Default, presentEverythingAsPercent);

        string flatModifierStringValue = GetParsedValue(Math.Abs(_mainStat.GetFlatModifier()), presentEverythingAsPercent);

        if (_mainStat.GetFlatModifier() > 0) result = $"({result} + {flatModifierStringValue})";
        else if (_mainStat.GetFlatModifier() < 0) result = $"({result} - {flatModifierStringValue})";
        
        if (_mainStat.GetMultiplierModifier() != 1f)
        {
            result += $" * {GetParsedValue(_mainStat.Default, true)}";
        }
        
        result = VisualTextParser.WrapInColor(result, _calculationColor);

        result += $" = {GetParsedValue(_mainStat.Value, presentEverythingAsPercent)}";
        
        _mainStatValueTextField.text = $"{_mainStatLabelLocKey.Localize()} - {result}";
    }

    private void UpdateDependencyStatValueDisplay()
    {
        string result = DEPENDENCY_STAT_VALUE_DISPLAY;
        
        result = GetParsedValue(_dependencyStat.Value, false) + VisualTextParser.GetStringSpriteFromData(_tagData.DependencyStatData);

        result += $" * {GetParsedValue(_mainStat.Value, true)}" + VisualTextParser.GetStringSpriteFromData(_tagData);
        
        result = VisualTextParser.WrapInColor(result, _calculationColor);
        
        result += $" = {GetParsedValue(_dependencyStat.Value * _mainStat.Value, false)}";
        
        _dependencyStatValueTextField.text = $"{_dependencyStatLabelLocKey.Localize()} {_tagData.ResultStatLocKey.Localize()} - {result}";
    }

    private string GetParsedValue(float value, bool presentAsPercent)
    {
        if (presentAsPercent) return $"{Math.Round(value) * 100:F0}%";

        return $"{value:F1}";
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        _mainStat.ValueChanged -= UpdateDisplayValues;
        if (_tagData.DependsOnStat) _dependencyStat.ValueChanged -= UpdateDisplayValues;
    } 
}