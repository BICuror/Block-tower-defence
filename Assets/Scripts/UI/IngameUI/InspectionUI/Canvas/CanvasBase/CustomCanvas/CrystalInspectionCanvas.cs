using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;
using UnityEngine.UI;
using Combat.Items;
using System.Linq;
using UnityEngine;
using TMPro;

namespace Combat.IngameUI.Inspection.InspectionCanvas
{
    public sealed class CrystalInspectionCanvas : ResizableInspectionCanvasBase
    {
        [Header("HeaderParameters")] 
        [SerializeField] private TextMeshProUGUI _rewardsAmountTextField;
        [SerializeField] private KeywordTooltipTagData _upgradeKeywordTooltipTagData;
    
        [Header("GlobalEffectDisplayPanels")]
        [SerializeField] private GlobalEffectTooltipInvokingPanelContainer _globalEffectTooltipInvokingPanelContainer;
        
        [Header("ContentLayoutGroup")]
        [SerializeField] private RectTransform _mainPanel;
        [SerializeField] private VerticalLayoutGroup _contentLayoutGroup;
        [SerializeField] private float _maxContentLayountGroupSize = 250f;
        [SerializeField] private float _additionalSize = 10f;
        
        [Header("Links")] 
        [SerializeField] private TooltipController _tooltipController;
        
        public void Initialize(Item item)
        {
            _rewardsAmountTextField.text = item.Charges.ToString();
            
            _contentLayoutGroup.childControlWidth = false;

            InitializeResizableInspectionPanel(item.GetComponent<InspectableObject>());
            CreateTooltips(item);
            RecalculatePreferredWidth();
    
            _contentLayoutGroup.childControlWidth = true;
            
            InitializeTooltipController();
        }
    
        private void CreateTooltips(Item item)
        {
            List<GlobalEffectData> sortedToggleEffectDatas = item.EffectDatas.OrderBy(data => data.EffectType == EffectType.Negative).ToList();
            
            _globalEffectTooltipInvokingPanelContainer.CopyParsersFromContainer(this);
            _globalEffectTooltipInvokingPanelContainer.Initialize(sortedToggleEffectDatas);
        }

        private void InitializeTooltipController()
        {
            TooltipParseTagDataContainer defaultTagDataContainer = new();
            defaultTagDataContainer.TagDatas.Add(_upgradeKeywordTooltipTagData);
            
            _tooltipController.CopyParsersFromContainer(this);
            _tooltipController.SetDefaultTooltipParseTagDataContainer(defaultTagDataContainer);
        }

        private void RecalculatePreferredWidth()
        {
            float maxSize = 0;
            
            foreach (GlobalEffectTooltipInvokingPanel globalEffectTooltipInvokingPanel in _globalEffectTooltipInvokingPanelContainer.InstantiatedPanels)
            {
                float panelSize = globalEffectTooltipInvokingPanel.PreferredTextWidth;
    
                if (panelSize > maxSize)
                {
                    maxSize = panelSize;
                }
            }
            
            if (maxSize > _maxContentLayountGroupSize) maxSize = _maxContentLayountGroupSize;
    
            _mainPanel.GetComponent<LayoutElement>().preferredWidth = maxSize + _additionalSize;
        }
    }
}