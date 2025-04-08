using UnityEngine;
using TMPro;

public sealed class InspectionKeywordSubpanel : MonoBehaviour
{
    [SerializeField] private InspectionSubpanelHeader _header;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    
    public void Initialize(KeywordTooltipTagData tagData)
    {
        _header.SetTagData(tagData);
        _descriptionTextField.text = tagData.Description;
    }
}