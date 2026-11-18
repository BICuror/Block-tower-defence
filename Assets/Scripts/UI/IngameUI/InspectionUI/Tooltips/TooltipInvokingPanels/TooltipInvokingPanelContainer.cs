using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;
using UnityEngine;

namespace Combat.IngameUI.Inspection
{
    public abstract class TooltipInvokingPanelContainer<TPanel, TPanelInitializationData> : ParsableTextContainer where TPanel : TooltipInvokingPanel
    {
        [SerializeField] protected TooltipController TooltipManager;

        [Header("Tooltips")] 
        [SerializeField] private TPanel _statTooltipInvokingPanelPrefab;
        [SerializeField] private Transform _statTooltipContainer;

        private List<TPanel> _instantiatedPanels = new();
        
        public IReadOnlyList<TPanel> InstantiatedPanels => _instantiatedPanels;

        protected void InitializeTooltipInvokingPanels(IEnumerable<TPanelInitializationData> panelInitializationData)
        {
            ClearAllPanels();
            
            foreach (TPanelInitializationData initializationData in panelInitializationData)
            {
                TPanel tooltipInvokingPanel = Instantiate(_statTooltipInvokingPanelPrefab, _statTooltipContainer);
                _instantiatedPanels.Add(tooltipInvokingPanel);

                InitializeTooltipInvokingPanel(tooltipInvokingPanel, initializationData);
                tooltipInvokingPanel.CopyParsersFromContainer(this);

                TooltipManager.SubscribeToTooltipInvokingPanel(tooltipInvokingPanel);

                tooltipInvokingPanel.TooltipOpened += OnPanelOpened;
                tooltipInvokingPanel.TooltipClosed += OnPanelClosed;
            }
        }

        protected virtual void OnPanelOpened(TooltipInvokingPanel panel) {}

        protected virtual void OnPanelClosed(TooltipInvokingPanel panel) {}

        protected abstract void InitializeTooltipInvokingPanel(TPanel panel, TPanelInitializationData initializationData);

        private void ClearAllPanels()
        {
            foreach (TPanel tooltipInvokingPanel in _instantiatedPanels)
            {
                Destroy(tooltipInvokingPanel.gameObject);
            }
            
            _instantiatedPanels.Clear();
        }
    }
}