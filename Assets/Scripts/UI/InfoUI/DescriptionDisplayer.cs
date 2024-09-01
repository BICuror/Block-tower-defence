using TMPro;
using UnityEngine;

public sealed class DescriptionDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void DisplayDescription(InspectableObject inspectable)
    {
        _nameText.SetText(inspectable.Name, true);
        _descriptionText.SetText(inspectable.Description, true);
    }
}
