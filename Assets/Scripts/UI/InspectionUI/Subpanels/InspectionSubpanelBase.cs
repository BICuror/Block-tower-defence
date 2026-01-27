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
    private bool _initialized;
    
    private void Awake()
    {
        _mainGroup = GetComponent<CanvasGroup>();
        _initialized = true;
        Enable();
    }
        
    protected void SetTagData(TooltipTagData tagData)
    {
        _descriptionTextField.text = _tooltipTextParser.GetTagDescription(tagData);
        _iconImage.sprite = tagData.IconSprite;
    }
    
    private void Enable()
    {
        if (!_initialized || !gameObject) return;
        
        _mainGroup.alpha = 0f;
        _mainGroup.DOKill();
        _mainGroup.DOFade(1f, _fadeDuration);
    }

    public void Disable()
    {
        if (!_initialized || !gameObject) return;
        
        GetComponent<LayoutElement>().ignoreLayout = true;
        
        _mainGroup.DOKill();
        _mainGroup.DOFade(0f, _fadeDuration).OnComplete(DestroySubpanel);
    }

    private void DestroySubpanel()
    {
        if (!gameObject) return;
        
        _mainGroup.DOKill();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _initialized = false;
        _mainGroup.DOKill();
    }
}