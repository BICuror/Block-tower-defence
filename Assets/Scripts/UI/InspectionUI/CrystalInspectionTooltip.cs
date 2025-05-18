using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class CrystalInspectionTooltip : MonoBehaviour
{
    [Header("links")] 
    [SerializeField] private Slider _negativeSlider;
    [SerializeField] private Slider _positiveSlider;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private LayoutSizeController _layoutSizeController;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private GlobalEffectTooltip _entityModificatorTooltipPrefab;
    [SerializeField] private Transform _entityModificatorTooltipParent;
    [SerializeField] private TextMeshProUGUI _durationTextField;
    
    private Dictionary<GlobalEffectData, GlobalEffectTooltip> _crystalTooltips = new();
    
    private Inspectable _inspectable;

    public void SetInspectable(Inspectable inspectable)
    {
        _inspectable = inspectable;

        Item item = inspectable.GetComponent<Item>();
        
        item.RewardDatas.ForEach(modificatorData =>
        {
            if (_crystalTooltips.ContainsKey(modificatorData))
            {
                _crystalTooltips[modificatorData].IncreaseAmount();
            }
            else
            {
                GlobalEffectTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
                tooltip.SetEntityModificator(modificatorData);
                tooltip.TooltipClosed += _inspectionSubpanelsController.ClearAllSubpanels;
                tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
                
                _crystalTooltips.Add(modificatorData, tooltip);
            }
        });
        
        item.ToggleEffectDatas.ForEach(modificatorData =>
        {
            if (_crystalTooltips.ContainsKey(modificatorData))
            {
                _crystalTooltips[modificatorData].IncreaseAmount();
            }
            else
            {
                GlobalEffectTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
                tooltip.SetEntityModificator(modificatorData);
                tooltip.TooltipClosed += _inspectionSubpanelsController.ClearAllSubpanels;
                tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
                
                _crystalTooltips.Add(modificatorData, tooltip);
            }
        });

        _negativeSlider.value = item.Strength - item.Quality;
        _positiveSlider.value = item.Strength + item.Quality;

        _durationTextField.text = item.Duration.ToString();
        
        _layoutSizeController.RecalculateLayout();
    }
}