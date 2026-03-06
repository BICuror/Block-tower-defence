using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public sealed class EntityModificatorTooltipInvokingPanel : TooltipInvokingPanel
{
    [Header("UI Elements")] 
    [SerializeField] private List<CanvasGroup> _negativeCanvasGroups;
    [SerializeField] private string _additionalFrontText;
    [SerializeField] private Image _iconImage;
    
    [Header("Links")] 
    [SerializeField] private TextMeshProUGUI _amountTextField;
    [SerializeField] private CanvasGroup _amountGroup;

    private TooltipParseTagDataContainer _tagDataContainer = new();
    private EntityModificatorData _entityModificatorData;
    private int _itemAmount = 1;

    protected override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;

    public event Action<EntityModificatorData> ModificatorDataSelected;
    
    public void SetEntityModificator(EntityModificatorData entityModificatorData)
    {
        _entityModificatorData = entityModificatorData;
        
        _negativeCanvasGroups.ForEach(group => group.gameObject.SetActive(entityModificatorData.EffectType == EffectType.Negative));
        
        _iconImage.sprite = entityModificatorData.Icon;

        UpdateContentSizeFilters();
        
        TooltipOpened += (_) => ModificatorDataSelected?.Invoke(_entityModificatorData); 
    }
    
    public void SetAmount(int amount)
    {
        if (amount == 1) return;
        
        _itemAmount = amount;
        
        _amountGroup.gameObject.SetActive(true);
        _amountTextField.text = "x" + _itemAmount.ToString();
    }

    protected override void UpdateAllParsableText()
    {
        _tagDataContainer = TooltipDataParser.GetTooltipTagDataFromText(_entityModificatorData.GetDescription());
    }
}