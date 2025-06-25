using UnityEngine;
using Combat;
using TMPro;

public sealed class EntityTooltip : InspectionPanel
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;

    [Header("Links")] 
    [SerializeField] private TooltipEntityModificatorContainer _tooltipEntityModificatorContainer;
    [SerializeField] private TooltipStatContainer _tooltipStatContainer;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private LayoutSizeController _layoutSizeController;
    [SerializeField] private PointFollowerUI _pointFollowerUI;
    
    private Inspectable _inspectable;

    private void Start()
    {
        _tooltipStatContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipStatContainer.TooltipClosed += ReturnToDefaultInspectionState;
        
        _tooltipEntityModificatorContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipEntityModificatorContainer.TooltipClosed += ReturnToDefaultInspectionState;
    }

    public void SetEntity(CombatEntity entity)
    {
        _inspectable = entity.ComponentsContainer.Get<Inspectable>();
        
        _nameTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Name);
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Description);
        _inspectionSubpanelsController.SetInspectedEntity(entity);
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));

        InitializeTooltipStatContainer(entity);
        InitializeEntityModificatorContainer(entity);

        _layoutSizeController.RecalculateLayout();
        
        _pointFollowerUI.SetTarget(entity.transform);
    }

    private void InitializeTooltipStatContainer(CombatEntity entity)
    {
        _tooltipStatContainer.SetInspectedEntity(entity);
    }    
    
    private void InitializeEntityModificatorContainer(CombatEntity entity)
    {
        _tooltipEntityModificatorContainer.SetInspectedEntity(entity);
    }

    private void ReturnToDefaultInspectionState()
    {
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
    }
}