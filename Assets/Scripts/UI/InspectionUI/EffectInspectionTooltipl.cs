using TMPro;
using UnityEngine;

public sealed class EffectInspectionTooltipl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    
    public void SetEffectName(string effectName)
    {
        _nameTextField.text = _tooltipTextParser.ParseTooltipText(effectName);
    }

    public void SetEffectDescription(string effectDescription)
    {
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(effectDescription);
        
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(effectDescription));
    }
}