using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;
using Combat.Entity.Modificators;
using System.Linq;
using UnityEngine;

namespace Combat.IngameUI.Inspection
{
    public sealed class EntityModificatorPanelContainer : TooltipInvokingPanelContainer<EntityModificatorTooltipInvokingPanel, EntityModificatorConfig>
    {
        [SerializeField] private EntityModificatorDisplayPanel _entityModificatorDisplayPanel;
        
        private CombatEntity _combatEntity;
        
        public void Initialize(CombatEntity entity)
        {
            _combatEntity = entity;
            _entityModificatorDisplayPanel.CopyParsersFromContainer(this);

            List<EntityModificatorConfig> modificatorConfigs = entity.ComponentsContainer.Get<EntityModificatorsContainer>().AppliedModificators.Where(modificatorData => modificatorData.ShowInInspector).ToList();

            modificatorConfigs = modificatorConfigs.OrderBy(data => data.EffectType == EffectType.Positive).ToList();
            
            InitializeTooltipInvokingPanels(modificatorConfigs);
        }
        
        protected override void OnPanelOpened(TooltipInvokingPanel panel)
        {
            _entityModificatorDisplayPanel.Enable(((EntityModificatorTooltipInvokingPanel)panel).EntityModificatorConfig);
        }

        protected override void OnPanelClosed(TooltipInvokingPanel panel)
        {
            _entityModificatorDisplayPanel.Disable();
        }

        protected override void InitializeTooltipInvokingPanel(EntityModificatorTooltipInvokingPanel panel, EntityModificatorConfig initializationData)
        {
            panel.SetAmount(_combatEntity.ComponentsContainer.Get<EntityModificatorsContainer>().GetEffectsAmount(initializationData));
            panel.SetEntityModificator(initializationData);
        }
    }
}