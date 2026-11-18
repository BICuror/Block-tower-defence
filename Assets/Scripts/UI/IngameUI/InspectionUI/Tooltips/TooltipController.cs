using System.Collections.Generic;
using UnityEngine;
using System;

namespace Combat.IngameUI.Inspection.Tooltips
{
    public sealed class TooltipController : ParsableTextContainer
    {
        [SerializeField] private Transform _subpanelsContainer;
        
        [SerializeField] private InspectionStatDetailsTooltip _statDetailsTooltipPrefab;
        [SerializeField] private InspectionKeywordTooltip _keywordTooltipPrefab;
        [SerializeField] private InspectionEffectTooltip _effectTooltipPrefab;
        [SerializeField] private InspectionStatTooltip _statTooltipPrefab;
        
        private TooltipParseTagDataContainer _defaultTooltipParseTagDataContainer;
        private List<TooltipPanelBase> _instantiatedTooltips = new();
        private CombatEntity _inspectedEntity;
    
        public event Action<TooltipTagData> CreatedTooltip;
        
        public void SetInspectedEntity(CombatEntity entity) => _inspectedEntity = entity;

        public void SetDefaultTooltipParseTagDataContainer(TooltipParseTagDataContainer defaultTooltipParseTagDataContainer)
        {
            _defaultTooltipParseTagDataContainer = defaultTooltipParseTagDataContainer;

            SetDefaultTooltipTagContainer();
        }

        public void SubscribeToTooltipInvokingPanel(TooltipInvokingPanel tooltipInvokingPanel)
        {
            tooltipInvokingPanel.TooltipOpened += SetTooltipInvokingPanel;
            tooltipInvokingPanel.TooltipClosed += _ => SetDefaultTooltipTagContainer();
        }

        private void SetTooltipInvokingPanel(TooltipInvokingPanel tooltipInvokingPanel) => SetTooltipTagContainer(tooltipInvokingPanel.TagDataContainer);
        
        private void SetDefaultTooltipTagContainer() => SetTooltipTagContainer(_defaultTooltipParseTagDataContainer);
        
        private void SetTooltipTagContainer(TooltipParseTagDataContainer tooltipParseTagDataContainer)
        {
            ClearAllTooltips();
    
            tooltipParseTagDataContainer.TagDatas.ForEach(tagData =>
            {
                if (tagData.InvokeTooltipPanel)
                {
                    if (tagData is StatTooltipTagData statTooltipTagData)
                    {
                        if (_inspectedEntity && _inspectedEntity.StatContainer.Has(Type.GetType(statTooltipTagData.AssociatedStatTypeName)))
                        {
                            InspectionStatDetailsTooltip statDetailsTooltip = Instantiate(_statDetailsTooltipPrefab, _subpanelsContainer);
                            InitializeBaseTooltip(statDetailsTooltip, tagData);
                            statDetailsTooltip.Initialize(_inspectedEntity, statTooltipTagData);
                            _instantiatedTooltips.Add(statDetailsTooltip);
                        }
                        else
                        {
                            InspectionStatTooltip statTooltip = Instantiate(_statTooltipPrefab, _subpanelsContainer);
                            InitializeBaseTooltip(statTooltip, tagData);
                            _instantiatedTooltips.Add(statTooltip);
                        }
                    }
                    else if (tagData is EffectTooltipTagData)
                    {
                        InspectionEffectTooltip effectTooltip = Instantiate(_effectTooltipPrefab, _subpanelsContainer);
                        InitializeBaseTooltip(effectTooltip, tagData);
                        _instantiatedTooltips.Add(effectTooltip);
                    }
                    else if (tagData is KeywordTooltipTagData)
                    {
                        InspectionKeywordTooltip keywordTooltip = Instantiate(_keywordTooltipPrefab, _subpanelsContainer);
                        InitializeBaseTooltip(keywordTooltip, tagData);
                    }
                    
                    CreatedTooltip?.Invoke(tagData);
                }
            });
        }
    
        private void InitializeBaseTooltip(TooltipPanelBase tooltipPanel, TooltipTagData tagData)
        {
            tooltipPanel.SetTagData(tagData);
            tooltipPanel.CopyParsersFromContainer(this);
            _instantiatedTooltips.Add(tooltipPanel);
            tooltipPanel.Enable();
        }
        
        public void ClearAllTooltips()
        {
            for (int i = 0; i < _instantiatedTooltips.Count; i++)
            {
                if (_instantiatedTooltips[i].gameObject != null) _instantiatedTooltips[i].Disable();
            }
            
            _instantiatedTooltips.Clear();
        }
    }
}