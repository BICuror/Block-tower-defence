using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Combat;
using System;
using TMPro;

public sealed class EntityModificatorPanelContainer : ParserableTextContainer
{
    [SerializeField] private EntityModificatorTooltipInvokingPanel _entityModificatorTooltipInvokingPanelPrefab;
    [SerializeField] private UIElementFadeAnimator _entityModificatorCanvasGroup;
    [SerializeField] private Transform _entityModificatorTooltipParent;
    [SerializeField] private CanvasGroup _negativeCanvasGroup;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _modificatorHeaderText;
    [SerializeField] private TextMeshProUGUI _modificatorDescriptionText;
    
    [Header("Details")]
    [SerializeField] private Transform _detailsContainer;
    [SerializeField] private TextMeshProUGUI _detailsTextFieldPrefab;
    [SerializeField] private Transform _detailsSeparatorPrefab;
    private List<GameObject> _createdDetailsUI = new();
    
    private List<EntityModificatorTooltipInvokingPanel> _entityModificatorPanels = new();
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;
    
    public void SetInspectedEntity(CombatEntity entity)
    {
        for (int i = 0; i < _entityModificatorPanels.Count; i++)
        {
            Destroy(_entityModificatorPanels[i].gameObject);
        }
        
        _entityModificatorPanels.Clear();
        
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.OrderBy(data => data.EffectType == EffectType.Positive).ToList().ForEach(modificatorData => 
        {
            if (modificatorData.ShowInInspector)
            {
                EntityModificatorTooltipInvokingPanel tooltipInvokingPanel = Instantiate(_entityModificatorTooltipInvokingPanelPrefab, _entityModificatorTooltipParent);
                
                _entityModificatorPanels.Add(tooltipInvokingPanel);
    
                tooltipInvokingPanel.SetAmount(entity.ComponentsContainer.Get<EntityModificatorsContainer>().GetModificatorsAmount(modificatorData));
                tooltipInvokingPanel.SetEntityModificator(modificatorData);
                tooltipInvokingPanel.CopyParsersFromContainer(this);
                
                tooltipInvokingPanel.TooltipOpened += (value) => TooltipOpened?.Invoke(value);
                tooltipInvokingPanel.TooltipClosed += () => TooltipClosed?.Invoke();

                tooltipInvokingPanel.ModificatorDataSelected += OpenEffectTooltip;
                tooltipInvokingPanel.TooltipClosed += CloseEffectTooltip;
            }
        });
    }
    

    private void OpenEffectTooltip(EntityModificatorData entityModificatorData)
    {
        InitializeDescription(entityModificatorData);
        
        _modificatorHeaderText.text = ParseTextByDefault(entityModificatorData.GetName());
        
        _negativeCanvasGroup.gameObject.SetActive(entityModificatorData.EffectType == EffectType.Negative);
        _entityModificatorCanvasGroup.Enable().Forget();
    }
    
    private void InitializeDescription(EntityModificatorData entityModificatorData)
    {
        string[] descriptions = GetDetailsDescriptions(entityModificatorData);
        
        _modificatorDescriptionText.text = ParseTextByDefault(descriptions[0]);
        
        _createdDetailsUI.ForEach(Destroy);

        _detailsContainer.gameObject.SetActive(descriptions.Length > 1);
        
        for (int i = 0; i < descriptions.Length - 1; i++)
        {
            if (i > 0) _createdDetailsUI.Add(Instantiate(_detailsSeparatorPrefab, _detailsContainer).gameObject);
            
            TextMeshProUGUI detailsField = Instantiate(_detailsTextFieldPrefab, _detailsContainer);
            detailsField.text = ParseTextByDefault("startTag" + descriptions[i + 1]);
            _createdDetailsUI.Add(detailsField.gameObject);
        }
    }
    
    private string[] GetDetailsDescriptions(EntityModificatorData entityModificatorData) => entityModificatorData.GetDescription().Split('/');

    private void CloseEffectTooltip()
    {
        _entityModificatorCanvasGroup.Disable().Forget();
    }
}