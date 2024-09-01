using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ParameterPanel : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _valueText;

    public void SetParameterData(Parameter parameter, GameObject inspectable)
    {
        _image.sprite = parameter.Icon;
        _nameText.text = parameter.Name;
        //_valueText.color = parameter.ValueColor;
        _valueText.text = parameter.GetValue(inspectable);
    }
}
