using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]

public abstract class InspectionPanel : MonoBehaviour
{
    private float _fadeDuration = 0.2f;
    private CanvasGroup _mainGroup;

    private void Awake()
    {
        _mainGroup = GetComponent<CanvasGroup>();
        Enable();
    }
    
    public void Enable()
    {
        _mainGroup.alpha = 0f;
        _mainGroup.DOFade(1f, _fadeDuration);
    }

    public void Disable()
    {
        _mainGroup.DOKill();
        _mainGroup.DOFade(0f, _fadeDuration).OnComplete(() => Destroy(gameObject));
    }
}