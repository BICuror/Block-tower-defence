using UnityEngine;
using System;
using System.Collections.Generic;
using Combat;

public sealed class InspectionSubpanelsController : MonoBehaviour
{
    [SerializeField] private Transform _subpanelsContainer;
    
    [SerializeField] private InspectionStatSubpanel _statSubpanelPrefab;
    [SerializeField] private InspectionStatDetailsSubpanel _statdetailsSubpanelPrefab;
    [SerializeField] private InspectionEffectSubPanel _effectSubpanelPrefab;
    [SerializeField] private InspectionKeywordSubpanel _keywordSubpanelPrefab;
    private List<InspectionSubpanelBase> _instantiatedTooltips = new();
    
    public CombatEntity _inspectedEntity;

    public void SetTooltipParser(TooltipParseTagDataContainer tooltipParseTagDataContainer)
    {
        ClearAllSubpanels();

        tooltipParseTagDataContainer.StatTagDatas.ForEach(tagData =>
        { 
            if (_inspectedEntity && _inspectedEntity.StatContainer.Has(Type.GetType(tagData.AssociatedStatTypeName))) 
            {
                Stat stat = _inspectedEntity.StatContainer.Get(Type.GetType(tagData.AssociatedStatTypeName));
                InspectionStatDetailsSubpanel statDetailsSubpanel = Instantiate(_statdetailsSubpanelPrefab, _subpanelsContainer);
                statDetailsSubpanel.Initialize(stat, tagData);
                _instantiatedTooltips.Add(statDetailsSubpanel);
            }
            else
            {
                InspectionStatSubpanel statSubpanel = Instantiate(_statSubpanelPrefab, _subpanelsContainer);
                statSubpanel.Initialize(tagData);
                _instantiatedTooltips.Add(statSubpanel);
            }
        });

        tooltipParseTagDataContainer.EffectTagDatas.ForEach(tagData =>
        {
            InspectionEffectSubPanel effectSubpanel = Instantiate(_effectSubpanelPrefab, _subpanelsContainer);
            effectSubpanel.Initialize(tagData);
            _instantiatedTooltips.Add(effectSubpanel);
        });

        tooltipParseTagDataContainer.KeywordTagDatas.ForEach(tagData =>
        {
            InspectionKeywordSubpanel keywordSubpanel = Instantiate(_keywordSubpanelPrefab, _subpanelsContainer);
            keywordSubpanel.Initialize(tagData);
            _instantiatedTooltips.Add(keywordSubpanel);
        });
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