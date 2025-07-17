using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public sealed class CrystalInspectionTooltip : InspectionPanel
{
    [Header("Links")] 
    [SerializeField] private TextMeshProUGUI _durationTextField;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private LayoutSizeController _layoutSizeController;
    [SerializeField] private GlobalEffectTooltip _entityModificatorTooltipPrefab;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private Transform _entityModificatorTooltipParent;
    [SerializeField] private PointFollowerUI _pointFollowerUI;
    
    private Dictionary<GlobalEffectData, GlobalEffectTooltip> _crystalTooltips = new();
    
    public void Initialize(Item item)
    {
        _crystalTooltips.Values.ToList().ForEach(tooltip => Destroy(tooltip.gameObject));
        _crystalTooltips.Clear();
        
        List<ToggleGlobalEffectData> sortedToggleEffectDatas = item.ToggleEffectDatas.OrderBy(item => item.EffectType == EffectType.Negative).ToList();

        ToggleGlobalEffectData startWaveToggleEffectData = sortedToggleEffectDatas.Find(effectData => effectData.InstanceItemTypeContainers.Exists(itemType => itemType.InstanceType == typeof(StartWaveGlobalToggleEffect)));

        if (startWaveToggleEffectData != null)
        {
            sortedToggleEffectDatas.Remove(startWaveToggleEffectData);
            CreateTooltip(startWaveToggleEffectData);
        }
        
        item.RewardDatas.OrderBy(data => data.EffectType == EffectType.Negative).ToList().ForEach(CreateTooltip);
        sortedToggleEffectDatas.ForEach(CreateTooltip);

        _durationTextField.text = item.Duration.ToString();
        
        _layoutSizeController.RecalculateLayout();
        
        _pointFollowerUI.SetTarget(item.transform);
    }

    private void CreateTooltip(GlobalEffectData globalEffectData)
    {
        if (_crystalTooltips.ContainsKey(globalEffectData))
        {
            _crystalTooltips[globalEffectData].IncreaseAmount();
        }
        else
        {
            GlobalEffectTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
            tooltip.SetEntityModificator(globalEffectData);
            tooltip.TooltipClosed += _inspectionSubpanelsController.ClearAllSubpanels;
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
                
            _crystalTooltips.Add(globalEffectData, tooltip);
        }
    }
}