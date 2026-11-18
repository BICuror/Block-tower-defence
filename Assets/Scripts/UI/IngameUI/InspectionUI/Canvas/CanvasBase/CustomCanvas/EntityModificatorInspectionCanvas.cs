using Combat.IngameUI.Inspection.Tooltips;
using System.Collections.Generic;
using Combat.Entity.Modificators;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace Combat.IngameUI.Inspection.InspectionCanvas
{
    public sealed class EntityModificatorInspectionCanvas : ResizableInspectionCanvasBase
    {   
        [SerializeField] private TooltipController _tooltipController;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _descriptionTextField;
        [SerializeField] private TextMeshProUGUI _nameTextField;
        [SerializeField] private Image _effectIcon;
    
        [Header("SetsDisplay")]
        [SerializeField] private CanvasGroup _setsCanvasGroup;
        [SerializeField] private EntitySetTooltipInvokingPanelContainer _entitySetTooltipInvokingPanelContainer;
        
        [Header("Details")] 
        [SerializeField] private Transform _detailsContainer;
        [SerializeField] private TextMeshProUGUI _detailsTextFieldPrefab;
        [SerializeField] private Transform _detailsSeparatorPrefab;
        private List<TextMeshProUGUI> _detailsTextFields = new();
        
        private EntityModificatorConfig _entityModificatorConfig;
    
        public void Initialize(EntityModificatorConfig entityModificatorConfig, BuildingEntity buildingEntity, Transform target)
        {
            _entityModificatorConfig = entityModificatorConfig;
            _effectIcon.sprite = entityModificatorConfig.Icon;
            
            InitializeDetails();
            
            InitializeResizableInspectionPanel(target.GetComponent<InspectableObject>());
            
            InitializeTooltipController();
            InitializeSetsDisplay(entityModificatorConfig, buildingEntity);
        }
    
        private void InitializeDetails()
        {
            int detailsCount = GetDetailsDescriptions().Length - 1;
    
            _detailsContainer.gameObject.SetActive(detailsCount > 0);
            
            for (int i = 0; i < detailsCount; i++)
            {
                if (i > 0) Instantiate(_detailsSeparatorPrefab, _detailsContainer);
                
                _detailsTextFields.Add(Instantiate(_detailsTextFieldPrefab, _detailsContainer));
            }
        }
    
        private void InitializeSetsDisplay(EntityModificatorConfig entityModificatorConfig, BuildingEntity buildingEntity)
        {
            _entitySetTooltipInvokingPanelContainer.CopyParsersFromContainer(this);
            _entitySetTooltipInvokingPanelContainer.Initialize(entityModificatorConfig.EntitySetTypes, buildingEntity);
        }
        
        protected override void UpdateAllParsableText()
        {
            _nameTextField.text = ParseTextByDefault(_entityModificatorConfig.GetName());
    
            string[] descriptions = GetDetailsDescriptions();
            
            _descriptionTextField.text = ParseTextByDefault(descriptions[0]);
    
            for (int i = 1; i < descriptions.Length; i++)
            {
                _detailsTextFields[i - 1].text = ParseTextByDefault("startTag" + descriptions[i]);
            }
        }
        
        private string[] GetDetailsDescriptions() => _entityModificatorConfig.GetDescription().Split('/');
        
        private void InitializeTooltipController()
        {
            _tooltipController.CopyParsersFromContainer(this);
            _tooltipController.SetDefaultTooltipParseTagDataContainer(TooltipDataParser.GetTooltipTagDataFromText(_entityModificatorConfig.GetDescription()));
        }
    }
}