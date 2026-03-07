using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public sealed class UIElementFadeAnimator : MonoBehaviour
{
    [Header("FadeAnimation")]
    [SerializeField] private CanvasGroup _mainGroup;
    [SerializeField] private float _fadeDuration = 0.2f;
    private bool _isActive;
    
    public bool IsActive => _isActive;
    
    private void Awake()
    {
        _mainGroup.interactable = false;
        _mainGroup.alpha = 0f;
    }
    
    public async UniTask Enable()
    {
        if (_isActive) return;
        _mainGroup.DOKill();
        
        _isActive = true;
        gameObject.SetActive(true);
        _mainGroup.interactable = true;
        await _mainGroup.DOFade(1f, _fadeDuration).SetUpdate(true).SetLink(_mainGroup.gameObject).AsyncWaitForCompletion();
    }
    
    public async UniTask Disable()
    {
        if (!_isActive) return;
        _mainGroup.DOKill();
        
        _isActive = false;
        _mainGroup.interactable = false;
        await _mainGroup.DOFade(0f, _fadeDuration).OnComplete(DisableGameObject).SetUpdate(true).SetLink(_mainGroup.gameObject).AsyncWaitForCompletion();
    }

    private void DisableGameObject()
    {
        if (gameObject) gameObject.SetActive(false);
    }
}
