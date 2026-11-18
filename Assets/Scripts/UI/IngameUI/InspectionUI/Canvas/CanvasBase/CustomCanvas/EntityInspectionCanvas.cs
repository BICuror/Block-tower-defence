using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.Entity.Sets;
using UI.CustomDropdown;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;
using UI;

namespace Combat.IngameUI.Inspection.InspectionCanvas
{
    public sealed class EntityInspectionCanvas : InspectionCanvasBase
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
    
        [Header("Sets")]
        [SerializeField] private EntitySetTooltipInvokingPanelContainer _entitySetTooltipInvokingPanelContainer;
        
        [Header("Links")] 
        [SerializeField] private EntityModificatorPanelContainer _entityModificatorPanelContainer;
        [SerializeField] private List<ScrollMaxHeightController> _scrollMaxHeightControllers;
        [SerializeField] private TooltipController _tooltipController; 
        [SerializeField] private StatTooltipInvokingPanelContainer _statTooltipInvokingPanelContainer;
        
        private CombatEntity _combatEntity;
        
        public CombatEntity InspectedEntity => _combatEntity;
    
        private void Start()
        {
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
            InitializeSetPanelContainer();
            UpdateHealthBar();
            
            _scrollMaxHeightControllers.ForEach(controller => controller.UpdateHeight().Forget());
            
            string parsedDescription = ParseByReplacebleData(Inspectable.Description);
            _tooltipController.SetDefaultTooltipParseTagDataContainer(TooltipDataParser.GetTooltipTagDataFromText(parsedDescription));
        }
        
        private void InitializeTooltipController()
        {
            _tooltipController.CopyParsersFromContainer(this);
            _tooltipController.SetInspectedEntity(_combatEntity);
        }
        
        protected override void UpdateAllParsableText()
        {
            _nameTextField.text = ParseTextByDefault(Inspectable.Name);
            _descriptionTextField.text = ParseTextByDefault(Inspectable.Description);
        }
        
        private void InitializeEntityModificatorsPanelContainer()
        {
            _entityModificatorPanelContainer.CopyParsersFromContainer(this);
            _entityModificatorPanelContainer.Initialize(_combatEntity);
        }
        
        private void InitializeStatPanelContainer()
        {
            _statTooltipInvokingPanelContainer.CopyParsersFromContainer(this);
            _statTooltipInvokingPanelContainer.Initialize(_combatEntity);
        }
        
        public void InitializeSetPanelContainer()
        {
            bool hasSets = _combatEntity.ComponentsContainer.Has<EntitySetContainer>();
            
            _entitySetTooltipInvokingPanelContainer.gameObject.SetActive(hasSets);
            
            if (!hasSets) return;
            
            _entitySetTooltipInvokingPanelContainer.CopyParsersFromContainer(this);
            _entitySetTooltipInvokingPanelContainer.Initialize(_combatEntity.ComponentsContainer.Get<EntitySetContainer>().GetAllContainedEntitySetTypes(), _combatEntity);
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
}