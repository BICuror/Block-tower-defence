using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public sealed class StatTooltip : BaseTooltip
{
    [Header("StatValueData")] 
    [SerializeField] private Color _defaultValueColor;
    [SerializeField] private Color _goodValueColor;
    [SerializeField] private Color _badValueColor;
    
    [Header("UI Elements")] 
    [SerializeField] private Image _statIconImage;
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private TextMeshProUGUI _statValueText;
    
    [Header("Links")]
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    private TooltipParseTagDataContainer _tagDataContainer;
    private Stat _stat;
    
    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public Stat AssiociatedStat => _stat;
    
    public Action StatValueChanged;
    
    public void SetStat(Stat stat)
    {
        _stat = stat;
        _stat.ValueChanged += HandleStatValueChange;
        
        StatTooltipTagData tagData = _allTagDataContainer.StatTagDatas.FirstOrDefault(tag => tag.AssociatedStatTypeName == _stat.GetType().Name);
        
        if (tagData == null) return;

        _tagDataContainer = new();
        _tagDataContainer.TagDatas.Add(tagData);

        _statIconImage.sprite = tagData.IconSprite;
        _statNameText.text = _tooltipTextParser.GetTagHeaderWithoutIcon(tagData);
        
        UpdateDisplayedStatValue();
    }

    private void UpdateDisplayedStatValue()
    {
        _statValueText.text = Math.Round(_stat.Value, 2).ToString();

        bool badValue = (_stat.Value < _stat.Default && !_stat.LowValueIsGood) || (_stat.Value > _stat.Default && _stat.LowValueIsGood);
        
        if (_stat.Value == _stat.Default) _statValueText.color = _defaultValueColor;
        else if (badValue) _statValueText.color = _badValueColor;
        else _statValueText.color = _goodValueColor;
    }

    private void HandleStatValueChange(float _)
    {
        UpdateDisplayedStatValue();
        StatValueChanged?.Invoke();
    }
    
    private void OnDisable()
    {
        if (_stat != null) _stat.ValueChanged -= HandleStatValueChange;
    }
}