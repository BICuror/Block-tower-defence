using System.Collections.Generic;
using UnityEngine;
using System;
using Combat;

public sealed class InspectionTooltipController : ParserableTextContainer
{
    [SerializeField] private Transform _subpanelsContainer;
    
    [SerializeField] private InspectionStatDetailsTooltip _statDetailsTooltipPrefab;
    [SerializeField] private InspectionKeywordTooltip _keywordTooltipPrefab;
    [SerializeField] private InspectionEffectTooltip _effectTooltipPrefab;
    [SerializeField] private InspectionStatTooltip _statTooltipPrefab;
    
    private List<TooltipPanelBase> _instantiatedTooltips = new();
    private CombatEntity _inspectedEntity;
    
    public void SetInspectedEntity(CombatEntity entity) => _inspectedEntity = entity;
    
    public void SetTooltipTagContainer(TooltipParseTagDataContainer tooltipParseTagDataContainer)
    {
        ClearAllSubpanels();

        tooltipParseTagDataContainer.TagDatas.ForEach(tagData =>
        {
            if (tagData.InvokeTooltipPanel)
            {
                if (tagData is StatTooltipTagData statTooltipTagData)
                {
                    if (_inspectedEntity && _inspectedEntity.StatContainer.Has(Type.GetType(statTooltipTagData.AssociatedStatTypeName)))
                    {
                        Stat stat = _inspectedEntity.StatContainer.Get(
                            Type.GetType(statTooltipTagData.AssociatedStatTypeName));
                        InspectionStatDetailsTooltip statDetailsTooltip =
                            Instantiate(_statDetailsTooltipPrefab, _subpanelsContainer);
                        statDetailsTooltip.Initialize(stat, statTooltipTagData);
                        _instantiatedTooltips.Add(statDetailsTooltip);
                    }
                    else
                    {
                        InspectionStatTooltip statTooltip = Instantiate(_statTooltipPrefab, _subpanelsContainer);
                        statTooltip.Initialize(statTooltipTagData);
                        _instantiatedTooltips.Add(statTooltip);
                    }
                }
                else if (tagData is EffectTooltipTagData effectTooltipTagData)
                {
                    InspectionEffectTooltip effectTooltip = Instantiate(_effectTooltipPrefab, _subpanelsContainer);
                    effectTooltip.Initialize(effectTooltipTagData);
                    _instantiatedTooltips.Add(effectTooltip);
                }
                else if (tagData is KeywordTooltipTagData keywordTooltipTagData)
                {
                    InspectionKeywordTooltip keywordTooltip = Instantiate(_keywordTooltipPrefab, _subpanelsContainer);
                    keywordTooltip.Initialize(keywordTooltipTagData);
                    _instantiatedTooltips.Add(keywordTooltip);
                }

                _instantiatedTooltips[^1].CopyParsersFromContainer(this);
                _instantiatedTooltips[^1].Enable();
            }
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