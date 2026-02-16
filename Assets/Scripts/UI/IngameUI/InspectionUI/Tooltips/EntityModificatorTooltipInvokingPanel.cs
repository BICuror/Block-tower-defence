using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EntityModificatorTooltipInvokingPanel : TooltipInvokingPanel
{
    [Header("UI Elements")] 
    [SerializeField] private List<CanvasGroup> _negativeCanvasGroups;
    [SerializeField] private TextMeshProUGUI _modificatorNameText;
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    [SerializeField] private string _additionalFrontText;
    [SerializeField] private Image _iconImage;
    
    [Header("Links")] 
    [SerializeField] private TextMeshProUGUI _amountTextField;
    [SerializeField] private CanvasGroup _amountGroup;

    private TooltipParseTagDataContainer _tagDataContainer = new();
    private EntityModificatorData _entityModificatorData;
    private int _itemAmount = 1;

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
    
    public void SetEntityModificator(EntityModificatorData entityModificatorData)
    {
        _entityModificatorData = entityModificatorData;
        
        _negativeCanvasGroups.ForEach(group => group.gameObject.SetActive(entityModificatorData.EffectType == EffectType.Negative));
        
        _iconImage.sprite = entityModificatorData.Icon;

        UpdateContentSizeFilters();
    }
    
    public void SetAmount(int amount)
    {
        if (amount == 1) return;
        
        _itemAmount = amount;
        
        _amountGroup.gameObject.SetActive(true);
        _amountTextField.text = _itemAmount.ToString();
    }

    protected override void UpdateAllParsableText()
    {
        _modificatorNameText.text = ParseTextByDefault(_entityModificatorData.ModificatorName);
        _modificatorDescriptionText.text = _additionalFrontText + ParseTextByDefault(_entityModificatorData.ModificatorDescription);
        _tagDataContainer = TooltipDataParser.GetTooltipTagDataFromText(_entityModificatorData.ModificatorDescription);
    }
}