using UnityEngine;
using TMPro;

public sealed class EntityModificatorTooltip : BaseTooltip
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _modificatorNameText;
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [Header("Links")] 
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    
    private TooltipParseTagDataContainer _tagDataContainer = new();

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(EntityModificatorData entityModificatorData)
    {
        _modificatorNameText.text = _tooltipTextParser.ParseTooltipText(entityModificatorData.ModificatorName);
        _modificatorDescriptionText.text = _tooltipTextParser.ParseTooltipText(entityModificatorData.ModificatorDescription, false);
        
        _tagDataContainer = _tooltipDataParser.GetTooltipTagDataFromText(entityModificatorData.ModificatorDescription);
    }
}