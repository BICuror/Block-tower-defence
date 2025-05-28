using UnityEngine;
using TMPro;

public sealed class EntityModificatorTooltip : BaseTooltip
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [Header("Links")] 
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private CanvasGroup _negativeCanvasGroup;
    
    private TooltipParseTagDataContainer _tagDataContainer = new();

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(EntityModificatorData entityModificatorData, bool isNegative)
    {
        _negativeCanvasGroup.gameObject.SetActive(isNegative);
        
        _modificatorDescriptionText.text = _tooltipTextParser.ParseTooltipText(entityModificatorData.ModificatorDescription, false);
        
        _tagDataContainer = _tooltipDataParser.GetTooltipTagDataFromText(entityModificatorData.ModificatorDescription);
    }
}