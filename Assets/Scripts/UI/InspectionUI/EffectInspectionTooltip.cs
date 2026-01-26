using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltip : PointFollowingCanvasUIElement
{   
    [SerializeField] private RectTransform _mainPanelRectTransform;
    [SerializeField] private Image _effectIcon;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;

    protected override Vector2 DynamicOffset => new (MainRectTransform.sizeDelta.x / 2f - _mainPanelRectTransform.sizeDelta.x / 2f, 0f);

    public async UniTask SetSelectionOptionObject(SelectionOptionObject selectionOptionObject)
    {
        _nameTextField.text = selectionOptionObject.OptionName;
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(selectionOptionObject.OptionDescription, false);
        _effectIcon.sprite = selectionOptionObject.Icon;
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(selectionOptionObject.OptionDescription));
        SetTarget(selectionOptionObject.transform);
    }
}