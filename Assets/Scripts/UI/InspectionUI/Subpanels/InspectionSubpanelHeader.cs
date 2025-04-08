using UnityEngine;
using TMPro;

public sealed class InspectionSubpanelHeader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerTextField;
    
    public void SetTagData(TooltipTagData tagData)
    {
        _headerTextField.text = $"<sprite name={tagData.IconSprite.name}>{tagData.FinalText}";    
    }
}