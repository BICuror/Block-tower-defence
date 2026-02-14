using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using TMPro;

public abstract class InspectionSubpanelBase : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mainGroup;
    [SerializeField] private LayoutElement _layoutElement;
    
    [Header("UI Elements")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] protected TooltipTextParser _tooltipTextParser;
    private float _fadeDuration = 0.1f;
    
    private void Awake() => Enable();
        
    protected void SetTagData(TooltipTagData tagData)
    {
        _descriptionTextField.text = _tooltipTextParser.GetTagDescription(tagData);
        _iconImage.sprite = tagData.IconSprite;
    }
    
    private void Enable()
    {
        _mainGroup.alpha = 0f;
        _mainGroup.DOKill();
        _mainGroup.DOFade(1f, _fadeDuration).SetLink(_mainGroup.gameObject);
    }

    public void Disable()
    {
        GetComponent<LayoutElement>().ignoreLayout = true;
        
        _mainGroup.DOKill();
        _mainGroup.DOFade(0f, _fadeDuration).SetLink(_mainGroup.gameObject).OnComplete(DestroySubpanel);
    }

    private void DestroySubpanel()
    {
        if (gameObject) Destroy(gameObject);
    }
}