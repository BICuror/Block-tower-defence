using Combat.IngameUI.Inspection.Tooltips;
using Combat.IngameUI.Inspection.Sets;
using System.Collections.Generic;
using Combat.Entity.Sets;
using UnityEngine;

namespace Combat.IngameUI.Inspection
{
    public sealed class EntitySetTooltipInvokingPanelContainer : TooltipInvokingPanelContainer<EntitySetTooltipInvokingPanel, EntitySetType>
    {
        [Header("DisplayPanel")]   
        [SerializeField] private SetDisplayPanel _setDisplayPanel;
        private CombatEntity _currentEntity;
    
        public void Initialize(IEnumerable<EntitySetType> entitySetTypes, CombatEntity entity)
        {
            _currentEntity = entity;
            _setDisplayPanel.CopyParsersFromContainer(this);
            
            InitializeTooltipInvokingPanels(entitySetTypes);
        }

        protected override void OnPanelOpened(TooltipInvokingPanel panel)
        {
            _setDisplayPanel.Enable(((EntitySetTooltipInvokingPanel)panel).EntitySetType, _currentEntity);
        }
        
        protected override void OnPanelClosed(TooltipInvokingPanel panel)
        {
            _setDisplayPanel.Disable();
        }

        protected override void InitializeTooltipInvokingPanel(EntitySetTooltipInvokingPanel panel, EntitySetType initializationData)
        {
            panel.SetEntitySetType(initializationData, _currentEntity);
        }
    }
}