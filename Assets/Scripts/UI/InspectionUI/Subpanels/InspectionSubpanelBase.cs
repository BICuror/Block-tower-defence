using UnityEngine.UI;
using UnityEngine;
using TMPro;

public abstract class InspectionSubpanelBase : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private Image _iconImage;
    [SerializeField] protected TooltipTextParser _tooltipTextParser;
    
    protected void SetTagData(TooltipTagData tagData)
    {
        _descriptionTextField.text = _tooltipTextParser.GetTagDescription(tagData);
        _iconImage.sprite = tagData.IconSprite;
    }
}