using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using TMPro;

public sealed class CrystalInspectionTooltip : InspectionPanelBase
{
    [Header("HeaderParameters")] 
    [SerializeField] private TextMeshProUGUI _rewardsAmountTextField;
    [SerializeField] private CanvasGroup _topCanvasGroup;

    [Header("Links")] 
    [SerializeField] private RectTransform _mainPanel;
    [SerializeField] private GlobalEffectTooltipInvokingPanel _entityModificatorTooltipInvokingPanelPrefab;
    [SerializeField] private InspectionTooltipController _inspectionTooltipController;
    [SerializeField] private VerticalLayoutGroup _contentLayoutGroup;
    [SerializeField] private float _maxContentLayountGroupSize = 250f;
    [SerializeField] private float _additionalSize = 10f;
    [SerializeField] private Transform _tooltipParent;
    
    private Dictionary<GlobalEffectData, GlobalEffectTooltipInvokingPanel> _crystalTooltips = new();
    
    public async UniTask Initialize(Item item)
    {
        _contentLayoutGroup.childControlWidth = false;
        
        InitializeInspectionPanelBase(item.GetComponent<InspectableObject>());
        _inspectionTooltipController.CopyParsersFromContainer(this);
        
        CreateTooltips(item);

        _rewardsAmountTextField.text = item.Charges.ToString();
        _topCanvasGroup.gameObject.SetActive(!item.EffectDatas.Exists(effectData => effectData.InstanceItemTypeContainers.Exists(itemType => itemType.InstanceType == typeof(StartWaveGlobalToggleEffect))));

        float maxSize = 0;
        
        foreach (GlobalEffectTooltipInvokingPanel globalEffectTooltipInvokingPanel in _crystalTooltips.Values)
        {
            float panelSize = globalEffectTooltipInvokingPanel.PreferredTextWidth;

            if (panelSize > maxSize)
            {
                maxSize = panelSize;
                Debug.Log(maxSize);
            }
        }
        
        if (maxSize > _maxContentLayountGroupSize) maxSize = _maxContentLayountGroupSize;

        _mainPanel.GetComponent<LayoutElement>().preferredWidth = maxSize + _additionalSize;

        _contentLayoutGroup.childControlWidth = true;
    }

    private void CreateTooltips(Item item)
    {
        List<GlobalEffectData> sortedToggleEffectDatas = item.EffectDatas.OrderBy(data => data.EffectType == EffectType.Negative).ToList();
        
        sortedToggleEffectDatas.ForEach(CreateTooltip);
    }
    
    private void CreateTooltip(GlobalEffectData globalEffectData)
    {
        GlobalEffectTooltipInvokingPanel tooltipInvokingPanel = Instantiate(_entityModificatorTooltipInvokingPanelPrefab, _tooltipParent);
        _crystalTooltips.Add(globalEffectData, tooltipInvokingPanel);
        
        tooltipInvokingPanel.SetEntityModificator(globalEffectData);
        tooltipInvokingPanel.CopyParsersFromContainer(this);
        
        tooltipInvokingPanel.TooltipClosed += _inspectionTooltipController.ClearAllSubpanels;
        tooltipInvokingPanel.TooltipOpened += _inspectionTooltipController.SetTooltipTagContainer;
    }
}