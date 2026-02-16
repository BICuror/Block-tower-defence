using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Combat;
using System;
using TMPro;

public sealed class EntityTooltip : InspectionPanelBase
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    [Header("Priority")] 
    [SerializeField] private CanvasGroup _priorityDropdownGroup;
    [SerializeField] private CustomDropdown _priorityDropdown;

    [Header("Links")] 
    [SerializeField] private EntityModificatorPanelContainer _entityModificatorPanelContainer;
    [SerializeField] private List<ScrollMaxHeightController> _scrollMaxHeightControllers;
    [SerializeField] private InspectionTooltipController _inspectionTooltipController;
    [SerializeField] private StatPanelContainer _statPanelContainer;
    
    private CombatEntity _combatEntity;

    private void Start()
    {
        _statPanelContainer.TooltipOpened += _inspectionTooltipController.SetTooltipTagContainer;
        _statPanelContainer.TooltipClosed += ReturnToDefaultInspectionState;
        
        _entityModificatorPanelContainer.TooltipOpened += _inspectionTooltipController.SetTooltipTagContainer;
        _entityModificatorPanelContainer.TooltipClosed += ReturnToDefaultInspectionState;
        
        _priorityDropdown.SelectedValueUpdated += OnPriorityDropdownValueChanged;
    }

    public void Initialize(CombatEntity entity)
    {
        _combatEntity = entity;
        
        InitializeInspectionPanelBase(entity.ComponentsContainer.Get<InspectableObject>());

        InitializeTooltipController();
        
        InitializeEntityModificatorsPanelContainer();
        InitializeStatPanelContainer();
        InitializePriorityDropdown();
        
        _scrollMaxHeightControllers.ForEach(controller => controller.UpdateHeight().Forget());

        ReturnToDefaultInspectionState();
    }
    
    private void InitializeTooltipController()
    {
        _inspectionTooltipController.CopyParsersFromContainer(this);
        _inspectionTooltipController.SetInspectedEntity(_combatEntity);
    }
    
    protected override void UpdateAllParsableText()
    {
        _nameTextField.text = ParseTextByDefault(Inspectable.Name);
        _descriptionTextField.text = ParseTextByDefault(Inspectable.Description);
    }
    
    private void InitializeEntityModificatorsPanelContainer()
    {
        _entityModificatorPanelContainer.CopyParsersFromContainer(this);
        _entityModificatorPanelContainer.SetInspectedEntity(_combatEntity);
    }
    
    private void InitializeStatPanelContainer()
    {
        _statPanelContainer.CopyParsersFromContainer(this);
        _statPanelContainer.SetInspectedEntity(_combatEntity);
    }
    
    private void InitializePriorityDropdown()
    {
        if (_combatEntity.TryGetComponent(out AreaManager areaManager) && areaManager.HasPriority)
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
        _combatEntity.ComponentsContainer.Get<AreaManager>().SetPriorityType((AreaEntityDetectorPriorityType)dropDownValue);
    }

    private void ReturnToDefaultInspectionState()
    {
        _inspectionTooltipController.SetTooltipTagContainer(TooltipDataParser.GetTooltipTagDataFromText(Inspectable.Description));
    }
}