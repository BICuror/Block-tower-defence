using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public sealed class StatTooltipInvokingPanel : TooltipInvokingPanel
{
    [Header("StatValueData")] 
    [SerializeField] private Color _defaultValueColor;
    [SerializeField] private Color _goodValueColor;
    [SerializeField] private Color _badValueColor;
    
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI _statValueText;
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private Image _statIconImage;
    
    [Header("Links")]
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    private TooltipParseTagDataContainer _tagDataContainer;
    private StatTooltipTagData _tagData;
    private Stat _stat;
    
    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetStat(Stat stat)
    {
        _stat = stat;
        _stat.ValueChanged += HandleStatValueChange;
        
        _tagData = _allTagDataContainer.StatTagDatas.FirstOrDefault(statTag => statTag.AssociatedStatTypeName == _stat.GetType().Name);

        _tagDataContainer = new() {TagDatas = new List<TooltipTagData>{_tagData}};

        _statIconImage.sprite = _tagData.IconSprite;
        
        UpdateDisplayedStatValue();
    }
    
    protected override void UpdateAllParsableText()
    {
        string resultText = VisualTextParser.GetTagHeaderWithoutIcon(_tagData);
        resultText = ReplaceableDataParser.ParseReplaceableData(resultText);

        _statNameText.text = resultText;
    }

    private void UpdateDisplayedStatValue()
    {
        string statValue;

        if (_tagData.PresentAsMultiplier) statValue = $"{Math.Round(_stat.Value, 2) * 100:F0}%";
        else statValue = Math.Round(_stat.Value, 2).ToString();
        
        _statValueText.text = statValue;

        bool badValue = (_stat.Value < _stat.Default && !_tagData.LowValueIsGood) || (_stat.Value > _stat.Default && _tagData.LowValueIsGood);
        
        if (_stat.Value == _stat.Default) _statValueText.color = _defaultValueColor;
        else if (badValue) _statValueText.color = _badValueColor;
        else _statValueText.color = _goodValueColor;
    }

    private void HandleStatValueChange(float _) => UpdateDisplayedStatValue();
    
    private void OnDestroy()
    {
        if (_stat != null) _stat.ValueChanged -= HandleStatValueChange;
        
        base.OnDestroy();
    }
}