using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]

public abstract class InspectionPanel : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] private CanvasGroup _mainGroup;
    private float _fadeDuration = 0.2f;
    private bool _initialized;

    public Action Closed;
    
    public void Enable()
    {
        _initialized = false;
        gameObject.SetActive(true);
        _mainGroup.DOKill();
        
        _mainGroup.DOFade(1f, _fadeDuration).OnComplete(() => _initialized = true);
    }

    public void Disable()
    {
        _mainGroup.DOKill();
        _mainGroup.DOFade(0f, _fadeDuration).OnComplete(() => gameObject.SetActive(false));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_initialized) return;
        
        Closed?.Invoke();
        
        Disable();
    }

    private void OnDestroy()
    {
        _mainGroup.DOKill();
    }
}