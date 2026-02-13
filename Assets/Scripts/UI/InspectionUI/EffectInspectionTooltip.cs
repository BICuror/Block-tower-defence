using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltip : PointFollowingCanvasUIElement
{   
    [SerializeField] private RectTransform _mainPanelRectTransform;
    [SerializeField] private Image _effectIcon;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    
    [Header("Parsers")]
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;

    protected override Vector2 DynamicOffset => new (MainRectTransform.sizeDelta.x / 2f - _mainPanelRectTransform.sizeDelta.x / 2f, 0f);

    public void Initialize(EntityModificatorData entityModificatorData, Transform target)
    {
        _nameTextField.text = entityModificatorData.ModificatorName;
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(entityModificatorData.ModificatorDescription, false);
        _effectIcon.sprite = entityModificatorData.Icon;
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(entityModificatorData.ModificatorDescription));
        SetTarget(target);
    }
}