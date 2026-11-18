using System.Collections.Generic;
using Combat.Entity.Modificators;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Combat.IngameUI.Inspection.Tooltips
{
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
        private EntityModificatorConfig _entityModificatorConfig;
        private int _itemAmount = 1;
    
        public override TooltipParseTagDataContainer TagDataContainer => _tagDataContainer;
        public EntityModificatorConfig EntityModificatorConfig => _entityModificatorConfig;
        
        public void SetEntityModificator(EntityModificatorConfig entityModificatorConfig)
        {
            _entityModificatorConfig = entityModificatorConfig;
            
            _negativeCanvasGroups.ForEach(group => group.gameObject.SetActive(entityModificatorConfig.EffectType == EffectType.Negative));
            
            _iconImage.sprite = entityModificatorConfig.Icon;
    
            UpdateContentSizeFilters();
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
            _tagDataContainer = TooltipDataParser.GetTooltipTagDataFromText(_entityModificatorConfig.GetDescription());
        }
    }
}