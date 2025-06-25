using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Combat;
using System;

public sealed class TooltipEntityModificatorContainer : MonoBehaviour
{
    [SerializeField] private Transform _entityModificatorTooltipParent;
    [SerializeField] private EntityModificatorTooltip _entityModificatorTooltipPrefab;
    private List<EntityModificatorTooltip> _entityModificatorTooltips = new();
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;
    
    public void SetInspectedEntity(CombatEntity entity)
    {
        for (int i = 0; i < _entityModificatorTooltips.Count; i++)
        {
            Destroy(_entityModificatorTooltips[i].gameObject);
        }
        
        _entityModificatorTooltips.Clear();
        
        entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.OrderBy(data => data.EffectType == EffectType.Positive).ToList().ForEach(modificatorData => 
        {
            EntityModificatorTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);

            tooltip.SetEntityModificator(modificatorData);
            tooltip.TooltipOpened += (value) => TooltipOpened?.Invoke(value);
            tooltip.TooltipClosed += () => TooltipClosed?.Invoke();
            
            tooltip.SetAmount(entity.ComponentsContainer.Get<EntityModificatorsContainer>().GetModificatorsAmount(modificatorData));
            
            _entityModificatorTooltips.Add(tooltip);
        });
    }
}