using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltip : ResizableInspectionPanelBase
{   
    [SerializeField] private InspectionTooltipController _inspectionTooltipController;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private Image _effectIcon;

    [Header("Details")] 
    [SerializeField] private Transform _detailsContainer;
    [SerializeField] private TextMeshProUGUI _detailsTextFieldPrefab;
    [SerializeField] private Transform _detailsSeparatorPrefab;
    private List<TextMeshProUGUI> _detailsTextFields = new();
    
    private EntityModificatorData _entityModificatorData;

    public void Initialize(EntityModificatorData entityModificatorData, Transform target)
    {
        _entityModificatorData = entityModificatorData;
        _effectIcon.sprite = entityModificatorData.Icon;
        
        InitializeDetails();
        
        InitializeResizableInspectionPanel(target.GetComponent<InspectableObject>());
        
        InitializeInspectionTooltipController();
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
    
    protected override void UpdateAllParsableText()
    {
        _nameTextField.text = ParseTextByDefault(_entityModificatorData.GetName());

        string[] descriptions = GetDetailsDescriptions();
        
        _descriptionTextField.text = ParseTextByDefault(descriptions[0]);

        for (int i = 1; i < descriptions.Length; i++)
        {
            _detailsTextFields[i - 1].text = ParseTextByDefault("startTag" + descriptions[i]);
        }
    }
    
    private string[] GetDetailsDescriptions() => _entityModificatorData.GetDescription().Split('/');
    
    private void InitializeInspectionTooltipController()
    {
        _inspectionTooltipController.CopyParsersFromContainer(this);
        
        _inspectionTooltipController.SetTooltipTagContainer(TooltipDataParser.GetTooltipTagDataFromText(_entityModificatorData.GetDescription()));
    }
}