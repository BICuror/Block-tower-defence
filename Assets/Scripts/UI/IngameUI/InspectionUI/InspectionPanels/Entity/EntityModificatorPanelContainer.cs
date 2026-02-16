using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Combat;
using System;
using UnityEngine.Serialization;

public sealed class EntityModificatorPanelContainer : ParserableTextContainer
{
    [SerializeField] private EntityModificatorTooltipInvokingPanel _entityModificatorTooltipInvokingPanelPrefab;
    [SerializeField] private Transform _entityModificatorTooltipParent;
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
            }
        });
    }
}