using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public abstract class TooltipPanelBase : ParserableTextContainer
{
    [Header("UI Elements")] 
    [SerializeField] private UIElementFadeAnimator _uiElementFadeAnimator;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private Image _iconImage;
    private TooltipTagData _tooltipTagData;
    
    protected void SetTagData(TooltipTagData tagData)
    {
        _tooltipTagData = tagData;
        _iconImage.sprite = _tooltipTagData.IconSprite;
    }

    public void Enable() => _uiElementFadeAnimator.Enable().Forget();
    public void Disable()
    {
        GetComponent<LayoutElement>().ignoreLayout = true;
        _uiElementFadeAnimator.Disable().Forget();
    }

    protected override void UpdateAllParsableText()
    {
        _descriptionTextField.text = VisualTextParser.GetTagDescription(_tooltipTagData);
    }
}