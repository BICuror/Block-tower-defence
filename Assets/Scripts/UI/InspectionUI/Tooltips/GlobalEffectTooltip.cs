using UnityEngine;
using TMPro;

public sealed class GlobalEffectTooltip : BaseTooltip
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [Header("Links")] 
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    
    private TooltipParseTagDataContainer _tagDataContainer = new();

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(GlobalEffectData entityModificatorData)
    {
        _modificatorDescriptionText.text = _tooltipTextParser.ParseTooltipText(entityModificatorData.EffectDescription);
    }
}
