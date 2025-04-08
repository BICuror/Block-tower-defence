using UnityEngine;
using TMPro;

public sealed class InspectionEffectSubPanel : MonoBehaviour
{
    [SerializeField] private InspectionSubpanelHeader _header;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    public void Initialize(EffectTooltipTagData tagData)
    {
        _header.SetTagData(tagData);
        _descriptionTextField.text = tagData.Description;
    }
}