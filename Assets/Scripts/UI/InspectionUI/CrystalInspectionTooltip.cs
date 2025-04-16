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
    
    private Inspectable _inspectable;

    public void SetInspectable(Inspectable inspectable)
    {
        _inspectable = inspectable;

        Item item = inspectable.GetComponent<Item>();
        
        item.RewardDatas.ForEach(modificatorData =>
        {
            GlobalEffectTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
            tooltip.SetEntityModificator(modificatorData);
            tooltip.TooltipClosed += _inspectionSubpanelsController.ClearAllSubpanels;
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        });
        
        item.ToggleEffectDatas.ForEach(modificatorData =>
        {
            GlobalEffectTooltip tooltip = Instantiate(_entityModificatorTooltipPrefab, _entityModificatorTooltipParent);
            tooltip.SetEntityModificator(modificatorData);
            tooltip.TooltipClosed += _inspectionSubpanelsController.ClearAllSubpanels;
            tooltip.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        });
        

        _layoutSizeController.RecalculateLayout();
    }
}