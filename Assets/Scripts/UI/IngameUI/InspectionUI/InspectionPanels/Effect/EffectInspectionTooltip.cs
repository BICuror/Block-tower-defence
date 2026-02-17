using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltip : InspectionPanelBase
{   
    [SerializeField] private InspectionTooltipController _inspectionTooltipController;
    [SerializeField] private RectTransform _mainPanelRectTransform;
    [SerializeField] private RectTransform _mainRectTransform;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private Image _effectIcon;
    
    private EntityModificatorData _entityModificatorData;

    public void Initialize(EntityModificatorData entityModificatorData, Transform target)
    {
        _entityModificatorData = entityModificatorData;
        _effectIcon.sprite = entityModificatorData.Icon;
        
        SetDynamicOffsetProvider(GetDynamicOffset);
        InitializeInspectionPanelBase(target.GetComponent<InspectableObject>());
        
        InitializeInspectionTooltipController();
    }
    
    protected override void UpdateAllParsableText()
    {
        _nameTextField.text = ParseTextByDefault(_entityModificatorData.ModificatorName);
        _descriptionTextField.text = ParseTextByDefault(_entityModificatorData.ModificatorDescription);
    }

    private void InitializeInspectionTooltipController()
    {
        _inspectionTooltipController.CopyParsersFromContainer(this);
        _inspectionTooltipController.SetTooltipTagContainer(TooltipDataParser.GetTooltipTagDataFromText(_entityModificatorData.ModificatorDescription));
    }
    
    private Vector2 GetDynamicOffset() => new(_mainRectTransform.sizeDelta.x / 2f - _mainPanelRectTransform.sizeDelta.x / 2f, 0f);
}