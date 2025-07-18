using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class GlobalEffectTooltip : BaseTooltip
{
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI _modificationNameText;
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [SerializeField] private Image _iconImage;
    [Header("Links")] 
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private List<CanvasGroup> _negativeCanvasGroup;
    
    private TooltipParseTagDataContainer _tagDataContainer;

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(GlobalEffectData globalEffectData)
    {
        _tagDataContainer = _tooltipDataParser.GetTooltipTagDataFromText(globalEffectData.EffectDescription);

        _modificationNameText.text = globalEffectData.EffectName;
        _modificatorDescriptionText.text = _tooltipTextParser.ParseTooltipText(globalEffectData.EffectDescription, false);
        
        _negativeCanvasGroup.ForEach(group => group.gameObject.SetActive(globalEffectData.EffectType == EffectType.Negative));

        _iconImage.sprite = globalEffectData.Icon;
    }
}
