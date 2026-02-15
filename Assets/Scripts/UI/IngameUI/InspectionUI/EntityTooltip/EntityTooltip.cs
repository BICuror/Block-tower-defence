using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Combat;
using System;
using TMPro;

public sealed class EntityTooltip : PointFollowingCanvasUIElement
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;

    [Header("Priority")] 
    [SerializeField] private CanvasGroup _priorityDropdownGroup;
    [SerializeField] private CustomDropdown _priorityDropdown;

    [Header("Links")] 
    [SerializeField] private TooltipEntityModificatorContainer _tooltipEntityModificatorContainer;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private TooltipStatContainer _tooltipStatContainer;
    [SerializeField] private List<ScrollMaxHeightController> _scrollMaxHeightControllers;
    
    private Inspectable _inspectable;

    private void Start()
    {
        _tooltipStatContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipStatContainer.TooltipClosed += ReturnToDefaultInspectionState;
        
        _tooltipEntityModificatorContainer.TooltipOpened += _inspectionSubpanelsController.SetTooltipParser;
        _tooltipEntityModificatorContainer.TooltipClosed += ReturnToDefaultInspectionState;
        
        _priorityDropdown.SelectedValueUpdated += OnPriorityDropdownValueChanged;
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

        InitializePriorityDropdown();
        
        _scrollMaxHeightControllers.ForEach(controller => controller.UpdateHeight().Forget());
    }
    
    private void InitializePriorityDropdown()
    {
        if (_inspectable.TryGetComponent(out AreaManager areaManager) && areaManager.HasPriority)
        {
            _priorityDropdownGroup.gameObject.SetActive(true);

            List<AreaEntityDetectorPriorityType> priorityTypes = Enum.GetValues(typeof(AreaEntityDetectorPriorityType)).Cast<AreaEntityDetectorPriorityType>().ToList();
            
            List<CustomDropdownItemData> options = new List<CustomDropdownItemData>();
            
            priorityTypes.ForEach(priorityType => options.Add(new CustomDropdownItemData((int)priorityType, priorityType.ToString())));

            _priorityDropdown.SetItemDatas(options);
            _priorityDropdown.SelectItem((int)areaManager.CurrentPriorityType);
        }
        else
        {
            _priorityDropdownGroup.gameObject.SetActive(false);
        }
    }

    private void OnPriorityDropdownValueChanged(int dropDownValue)
    {
        _inspectable.GetComponent<AreaManager>().SetPriorityType((AreaEntityDetectorPriorityType)dropDownValue);
    }

    private void ReturnToDefaultInspectionState()
    {
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_inspectable.Description));
    }
}