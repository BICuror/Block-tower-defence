using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;
using Combat.Stats;
using UnityEngine;
using System;

namespace Combat.IngameUI.Inspection
{
    public sealed class StatTooltipInvokingPanelContainer : TooltipInvokingPanelContainer<StatTooltipInvokingPanel, Stat>
    {
        [Header("CustomTooltips")]
        [SerializeField] private List<StatTooltipInvokingPanel> _customStatTooltips;
        [SerializeField] private List<string> _customStatTypeNames;
        
        private void Awake()
        {
            InitializeCustomTooltips();
        }
    
        public void Initialize(CombatEntity entity)
        {
            EnableCustomTooltips(entity);
            InitializeStatTooltips(entity);
        }
        
        protected override void InitializeTooltipInvokingPanel(StatTooltipInvokingPanel panel, Stat initializationData)
        {
            panel.SetStat(initializationData);
        }
        
        private void InitializeCustomTooltips()
        {
            foreach (StatTooltipInvokingPanel statTooltipInvokingPanel in _customStatTooltips)
            {
                TooltipManager.SubscribeToTooltipInvokingPanel(statTooltipInvokingPanel);
            }
        }
        
        private void InitializeStatTooltips(CombatEntity entity)
        {
            List<Stat> entityStats = entity.StatContainer.GetAllStats();
    
            _customStatTypeNames.ForEach(typeName => entityStats.RemoveAll(stat => stat.GetType() == Type.GetType(typeName)));
            
            InitializeTooltipInvokingPanels(entityStats);
        }
        
        private void EnableCustomTooltips(CombatEntity entity)
        {
            _customStatTypeNames.ForEach(typeName =>
            {
                Type statType = Type.GetType(typeName);
                
                bool hasStat = entity.StatContainer.Has(statType);
    
                StatTooltipInvokingPanel statTooltipInvokingPanel = _customStatTooltips[_customStatTypeNames.IndexOf(typeName)];
                
                statTooltipInvokingPanel.gameObject.SetActive(hasStat);
                
                if (hasStat)
                {
                    statTooltipInvokingPanel.SetStat(entity.StatContainer.Get(statType));
                }
            });
        }
    }
}