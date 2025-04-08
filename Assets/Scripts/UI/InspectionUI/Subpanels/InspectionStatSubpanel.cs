using UnityEngine;
using TMPro;

public sealed class InspectionStatSubpanel : MonoBehaviour
{
    [SerializeField] private InspectionSubpanelHeader _header;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    public void Initialize(StatTooltipTagData tagData)
    {
        _header.SetTagData(tagData);
        _descriptionTextField.text = tagData.Description;
    }
}