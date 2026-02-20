using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class GlobalEffectTooltipInvokingPanel : TooltipInvokingPanel
{
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI _modificationNameText;
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [SerializeField] private Image _iconImage;
    [Header("Links")] 
    [SerializeField] private List<CanvasGroup> _negativeCanvasGroup;
    private TooltipParseTagDataContainer _tagDataContainer;
    private GlobalEffectData _globalEffectData;

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(GlobalEffectData globalEffectData)
    {
        _globalEffectData = globalEffectData;
        
        _negativeCanvasGroup.ForEach(group => group.gameObject.SetActive(globalEffectData.EffectType == EffectType.Negative));

        _iconImage.sprite = globalEffectData.Icon;
    }
    
    protected override void UpdateAllParsableText()
    {
        _modificationNameText.text = ParseTextByDefault(_globalEffectData.GetName());
        _modificatorDescriptionText.text = ParseTextByDefault(_globalEffectData.GetDescription());
        _tagDataContainer = TooltipDataParser.GetTooltipTagDataFromText(_globalEffectData.GetDescription());
    }
}
