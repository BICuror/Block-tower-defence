using UnityEngine;
using TMPro;

public sealed class GlobalEffectTooltip : BaseTooltip
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [Header("Links")] 
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private CanvasGroup _positiveCanvasGroup;
    [SerializeField] private CanvasGroup _negativeCanvasGroup;
    
    private TooltipParseTagDataContainer _tagDataContainer;

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(GlobalEffectData globalEffectData)
    {
        _tagDataContainer = _tooltipDataParser.GetTooltipTagDataFromText(globalEffectData.EffectDescription);
        
        _modificatorDescriptionText.text = _tooltipTextParser.ParseTooltipText(globalEffectData.EffectDescription, false);
        
        _positiveCanvasGroup.gameObject.SetActive(globalEffectData.EffectType == EffectType.Positive);
        _negativeCanvasGroup.gameObject.SetActive(globalEffectData.EffectType == EffectType.Negative);
    }
}
