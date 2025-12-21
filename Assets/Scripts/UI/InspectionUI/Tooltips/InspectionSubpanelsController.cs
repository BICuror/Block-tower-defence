using System.Collections.Generic;
using UnityEngine;
using System;
using Combat;

public sealed class InspectionSubpanelsController : MonoBehaviour
{
    [SerializeField] private Transform _subpanelsContainer;
    
    [SerializeField] private InspectionStatSubpanel _statSubpanelPrefab;
    [SerializeField] private InspectionStatDetailsSubpanel _statdetailsSubpanelPrefab;
    [SerializeField] private InspectionEffectSubPanel _effectSubpanelPrefab;
    [SerializeField] private InspectionKeywordSubpanel _keywordSubpanelPrefab;
    private List<InspectionSubpanelBase> _instantiatedTooltips = new();
    private CombatEntity _inspectedEntity;

    public event Action TooltipsUpdated;
    
    public void SetInspectedEntity(CombatEntity entity) => _inspectedEntity = entity;
    
    public void SetTooltipParser(TooltipParseTagDataContainer tooltipParseTagDataContainer)
    {
        ClearAllSubpanels();

        tooltipParseTagDataContainer.TagDatas.ForEach(tagData =>
        {
            if (tagData is StatTooltipTagData statTooltipTagData)
            {
                if (_inspectedEntity && _inspectedEntity.StatContainer.Has(Type.GetType(statTooltipTagData.AssociatedStatTypeName))) 
                {
                    Stat stat = _inspectedEntity.StatContainer.Get(Type.GetType(statTooltipTagData.AssociatedStatTypeName));
                    InspectionStatDetailsSubpanel statDetailsSubpanel = Instantiate(_statdetailsSubpanelPrefab, _subpanelsContainer);
                    statDetailsSubpanel.Initialize(stat, statTooltipTagData);
                    _instantiatedTooltips.Add(statDetailsSubpanel);
                }
                else
                {
                    InspectionStatSubpanel statSubpanel = Instantiate(_statSubpanelPrefab, _subpanelsContainer);
                    statSubpanel.Initialize(statTooltipTagData);
                    _instantiatedTooltips.Add(statSubpanel);
                }
            }  
            else if (tagData is EffectTooltipTagData effectTooltipTagData)
            {
                InspectionEffectSubPanel effectSubpanel = Instantiate(_effectSubpanelPrefab, _subpanelsContainer);
                effectSubpanel.Initialize(effectTooltipTagData);
                _instantiatedTooltips.Add(effectSubpanel);
            }
            else if (tagData is KeywordTooltipTagData keywordTooltipTagData)
            {
                InspectionKeywordSubpanel keywordSubpanel = Instantiate(_keywordSubpanelPrefab, _subpanelsContainer);
                keywordSubpanel.Initialize(keywordTooltipTagData);
                _instantiatedTooltips.Add(keywordSubpanel);
            }
        });
        
        TooltipsUpdated?.Invoke();
    }
    
    public void ClearAllSubpanels()
    {
        for (int i = 0; i < _instantiatedTooltips.Count; i++)
        {
            _instantiatedTooltips[i].Disable();
        }
        
        _instantiatedTooltips.Clear();
    }
}