using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]

public abstract class InspectionPanel : MonoBehaviour, IPointerExitHandler
{
    private float _fadeDuration = 0.2f;
    private CanvasGroup _mainGroup;

    private void Awake()
    {
        _mainGroup = GetComponent<CanvasGroup>();
    }
    
    private void OnEnable()
    {
        _mainGroup.alpha = 0f;
        _mainGroup.DOFade(1f, _fadeDuration);
    }

    private void Disable()
    {
        _mainGroup.DOKill();
        _mainGroup.DOFade(0f, _fadeDuration);
        gameObject.SetActive(false);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Disable();
    }
}