using UnityEngine;
using Combat;
using Cysharp.Threading.Tasks;
using TMPro;

public sealed class EntityTooltip : PointFollowingCanvasUIElement
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;

    [Header("Links")] 
    [SerializeField] private TooltipEntityModificatorContainer _tooltipEntityModificatorContainer;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private TooltipStatContainer _tooltipStatContainer;
    
    private Inspectable _inspectable;

    private void Start()
    {
        _tooltipStatContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipStatContainer.TooltipClosed += ReturnToDefaultInspectionState;
        
        _tooltipEntityModificatorContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipEntityModificatorContainer.TooltipClosed += ReturnToDefaultInspectionState;
    }

    public async UniTask Initialize(CombatEntity entity)
    {
        _inspectable = entity.ComponentsContainer.Get<Inspectable>();
        
        _nameTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Name);
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(_inspectable.Description);
        _inspectionSubpanelsController.SetInspectedEntity(entity);
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));

        _tooltipStatContainer.SetInspectedEntity(entity);
        _tooltipEntityModificatorContainer.SetInspectedEntity(entity);
        
        SetTarget(entity.transform);
        await RebuildLayoutAndCalculateOffsets();
    }

    private void ReturnToDefaultInspectionState()
    {
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
    }
}