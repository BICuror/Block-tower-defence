using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Combat;
using System;
using TMPro;
using UnityEngine.UI;

public sealed class EntityTooltip : InspectionPanelBase
{
    [Header("HealthBar")] 
    [SerializeField] private TextMeshProUGUI _healthBarText;
    [SerializeField] private Gradient _healthBarGradient;
    [SerializeField] private Image _healthBarImage;
    
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

        _combatEntity.Health.Died += DisableOnEntityDeath;
        _combatEntity.Health.Damaged += UpdateHealthBar;
        _combatEntity.Health.Healed += UpdateHealthBar;
    }

    public void Initialize(CombatEntity entity)
    {
        _combatEntity = entity;
        
        InitializeInspectionPanelBase(entity.ComponentsContainer.Get<InspectableObject>());

        InitializeTooltipController();
        
        InitializeEntityModificatorsPanelContainer();
        InitializeStatPanelContainer();
        InitializePriorityDropdown();
        UpdateHealthBar();
        
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
            
            List<int> options = new List<int>();
            
            priorityTypes.ForEach(priorityType => options.Add((int)priorityType));

            _priorityDropdown.SetItemValues(options);
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

    private void UpdateHealthBar()
    {
        float healthPercent = _combatEntity.Health.GetHpPercent();
        
        _healthBarImage.color = _healthBarGradient.Evaluate(healthPercent);
        _healthBarImage.fillAmount = healthPercent;
        _healthBarText.text = $"{_combatEntity.Health.GetHp():F1} / {_combatEntity.Health.GetMaxHp():F1}";
    }

    private void DisableOnEntityDeath() => Disable().Forget();

    private void OnDestroy()
    {
        base.OnDestroy();
        
        _combatEntity.Health.Died -= DisableOnEntityDeath;
        _combatEntity.Health.Damaged -= UpdateHealthBar;
        _combatEntity.Health.Healed -= UpdateHealthBar;
    }
}