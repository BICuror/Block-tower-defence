using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public sealed class StatTooltip : BaseTooltip
{
    [Header("UI Elements")] 
    [SerializeField] private Image _statIconImage;
    [SerializeField] private TextMeshProUGUI _statNameText;
    [SerializeField] private TextMeshProUGUI _statValueText;
    [Header("Links")]
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
        
    private TooltipParseTagDataContainer _tagDataContainer = new();

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetStat(Stat stat)
    {
        StatTooltipTagData tagData = _allTagDataContainer.StatTagDatas.FirstOrDefault(tag => tag.AssociatedStatTypeName == stat.GetType().Name);
        
        if (tagData == null) return;

        _tagDataContainer.StatTagDatas.Add(tagData);

        _statIconImage.sprite = tagData.IconSprite;
        _statNameText.text = _tooltipTextParser.GetTagHeaderWithoutIcon(tagData);
        _statValueText.text = Math.Round(stat.Value, 2).ToString();
    }
}