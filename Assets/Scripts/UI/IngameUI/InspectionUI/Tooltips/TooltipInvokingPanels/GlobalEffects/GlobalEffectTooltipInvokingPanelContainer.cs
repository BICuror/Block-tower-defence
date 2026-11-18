using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;

namespace Combat.IngameUI.Inspection
{
    public sealed class GlobalEffectTooltipInvokingPanelContainer : TooltipInvokingPanelContainer<GlobalEffectTooltipInvokingPanel, GlobalEffectData>
    {
        public void Initialize(IEnumerable<GlobalEffectData> globalEffectConfigs)
        {
            InitializeTooltipInvokingPanels(globalEffectConfigs);
        }
        
        protected override void InitializeTooltipInvokingPanel(GlobalEffectTooltipInvokingPanel panel, GlobalEffectData initializationData)
        {
            panel.SetGlobalEffectConfig(initializationData);
        }
    }
}