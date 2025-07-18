using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(LayoutElement))]

public abstract class InspectionSubpanelBase : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private Image _iconImage;
    [SerializeField] protected TooltipTextParser _tooltipTextParser;
    private float _fadeDuration = 0.1f;
    private CanvasGroup _mainGroup;

    private void Awake()
    {
        _mainGroup = GetComponent<CanvasGroup>();
        Enable();
    }
        
    protected void SetTagData(TooltipTagData tagData)
    {
        _descriptionTextField.text = _tooltipTextParser.GetTagDescription(tagData);
        _iconImage.sprite = tagData.IconSprite;
    }
    
    private void Enable()
    {
        _mainGroup.alpha = 0f;
        _mainGroup.DOFade(1f, _fadeDuration);
    }

    public void Disable()
    {
        transform.position = transform.position;
        GetComponent<LayoutElement>().ignoreLayout = true;
        _mainGroup.DOKill();
        _mainGroup.DOFade(0f, _fadeDuration).OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        _mainGroup.DOKill();
    }
}