using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

public sealed class CrystalInspectionTooltip : PointFollowingCanvasUIElement
{
    [Header("HeaderParameters")] 
    [SerializeField] private TextMeshProUGUI _rewardsAmountTextField;
    [SerializeField] private TextMeshProUGUI _durationTextField;
    [SerializeField] private CanvasGroup _startWaveCanvasGroup;
    
    [Header("Links")] 
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private GlobalEffectTooltip _entityModificatorTooltipPrefab;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private Transform _tooltipParent;
    
    private Dictionary<GlobalEffectData, GlobalEffectTooltip> _crystalTooltips = new();
    
    public async UniTask Initialize(Item item)
    {
        CreateTooltips(item);

        _rewardsAmountTextField.text = item.RewardDatas.Count.ToString();
        _durationTextField.text = item.Duration.ToString();
        _startWaveCanvasGroup.gameObject.SetActive(item.ToggleEffectDatas.Exists(effectData => effectData.InstanceItemTypeContainers.Exists(itemType => itemType.InstanceType == typeof(StartWaveGlobalToggleEffect))));
        
        SetTarget(item.transform);
        await RebuildLayoutAndCalculateOffsets();
    }

    private void CreateTooltips(Item item)
    {
        List<ToggleGlobalEffectData> sortedToggleEffectDatas = item.ToggleEffectDatas.OrderBy(item => item.EffectType == EffectType.Negative).ToList();
        
        ToggleGlobalEffectData startWaveEffect = item.ToggleEffectDatas.Find(effectData => effectData.InstanceItemTypeContainers.Exists(itemType => itemType.InstanceType == typeof(StartWaveGlobalToggleEffect)));

        _startWaveCanvasGroup.gameObject.SetActive(startWaveEffect);

        if (startWaveEffect)
        {
            sortedToggleEffectDatas.Remove(startWaveEffect);
        }
        
        sortedToggleEffectDatas.ForEach(CreateTooltip);
    }
    
    private void CreateTooltip(GlobalEffectData globalEffectData)
    {
        if (_crystalTooltips.ContainsKey(globalEffectData))
        {
            _crystalTooltips[globalEffectData].IncreaseAmount();
        }
        else
        {
            GlobalEffectTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _tooltipParent);
            tooltip.SetEntityModificator(globalEffectData);
            tooltip.TooltipClosed += _inspectionSubpanelsController.ClearAllSubpanels;
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
                
            _crystalTooltips.Add(globalEffectData, tooltip);
        }
    }
}