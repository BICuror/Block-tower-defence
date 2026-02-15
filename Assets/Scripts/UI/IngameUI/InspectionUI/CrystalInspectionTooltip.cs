using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

public sealed class CrystalInspectionTooltip : PointFollowingCanvasUIElement
{
    [Header("HeaderParameters")] 
    [SerializeField] private TextMeshProUGUI _rewardsAmountTextField;
    [SerializeField] private CanvasGroup _topCanvasGroup;
    
    [Header("Links")] 
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private GlobalEffectTooltip _entityModificatorTooltipPrefab;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private Transform _tooltipParent;
    
    private Dictionary<GlobalEffectData, GlobalEffectTooltip> _crystalTooltips = new();
    
    public async UniTask Initialize(Item item)
    {
        CreateTooltips(item);

        _rewardsAmountTextField.text = item.Charges.ToString();
        _topCanvasGroup.gameObject.SetActive(!item.ToggleEffectDatas.Exists(effectData => effectData.InstanceItemTypeContainers.Exists(itemType => itemType.InstanceType == typeof(StartWaveGlobalToggleEffect))));
        
        SetTarget(item.transform);
    }

    private void CreateTooltips(Item item)
    {
        List<ToggleGlobalEffectData> sortedToggleEffectDatas = item.ToggleEffectDatas.OrderBy(item => item.EffectType == EffectType.Negative).ToList();
        
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