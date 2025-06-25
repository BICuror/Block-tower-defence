using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private PointFollowerUI _pointFollowerUI;
    private SelectionOptionObject _selectionOptionObject;

    public void SetSelectionOptionObject(SelectionOptionObject selectionOptionObject)
    {
        _selectionOptionObject = selectionOptionObject;
        
        _pointFollowerUI.SetTarget(selectionOptionObject.transform);
        SetEffectName(_selectionOptionObject.OptionName);
        SetEffectDescription(_selectionOptionObject.OptionDescription);
    }

    private void SetEffectName(string effectName)
    {
        _nameTextField.text = effectName;
    }
    
    private void SetEffectDescription(string effectDescription)
    {
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(effectDescription, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_selectionOptionObject.OptionDescription));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inspectionSubpanelsController.ClearAllSubpanels();
    }
}