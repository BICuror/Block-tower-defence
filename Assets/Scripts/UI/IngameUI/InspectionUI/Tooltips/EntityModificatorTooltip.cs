using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public sealed class EntityModificatorTooltip : BaseTooltip
{
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI _modificatorNameText;
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private string _additionalFrontText;
    [Header("Links")] 
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private List<CanvasGroup> _negativeCanvasGroups;
    
    private TooltipParseTagDataContainer _tagDataContainer = new();

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(EntityModificatorData entityModificatorData)
    {
        _negativeCanvasGroups.ForEach(group => group.gameObject.SetActive(entityModificatorData.EffectType == EffectType.Negative));
        
        _iconImage.sprite = entityModificatorData.Icon;
        _modificatorNameText.text = entityModificatorData.ModificatorName;
        _modificatorDescriptionText.text = _additionalFrontText + _tooltipTextParser.ParseTooltipText(entityModificatorData.ModificatorDescription, false);
        
        _tagDataContainer = _tooltipDataParser.GetTooltipTagDataFromText(entityModificatorData.ModificatorDescription);

        UpdateContentSizeFilters();
    }
}