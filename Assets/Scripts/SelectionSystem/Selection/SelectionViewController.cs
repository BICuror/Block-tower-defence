using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System;
using TMPro;

public sealed class SelectionViewController : MonoBehaviour
{
    [Inject] private PostProcessingController _postProcessingController;

    [Header("Indicators")] 
    [SerializeField] private float _postExposureDimStrength = 0.1f;
    [SerializeField] private List<SelectionIndicatorContainers> _selectionIndicatorContainers;
    
    [SerializeField] private Transform _selectionViewGraphics;
    [SerializeField] private float _selectionViewTransitionDuration = 1f;
    [SerializeField] private AnimationCurve _selectionViewAppearAnimationCurve;
    [SerializeField] private AnimationCurve _selectionViewDisappearAnimationCurve;
    
    [SerializeField] public SelectionManager _selectionManager;
    private SelectionIndicatorContainers _currentSelectionIndicatorContainer;

    private void Awake()
    {
        _selectionManager.SelectionStarted += EnableSelectionView;
        _selectionManager.SelectionEnded += DisableSelectionView;
        
        _selectionManager.SelectionStepStarted += () => EnableSelectionIndicator().Forget();
        _selectionManager.SelectionStepEnded += () => DisableSelectionIndicator().Forget();
    }

    private void EnableSelectionView()
    {
        _selectionViewGraphics.DOKill();
        _selectionViewGraphics.DOScale(Vector3.one, _selectionViewTransitionDuration).SetEase(_selectionViewAppearAnimationCurve);
        _selectionViewGraphics.gameObject.SetActive(true);
    }
    
    private void DisableSelectionView()
    {
        _selectionViewGraphics.DOKill();
        _selectionViewGraphics.DOScale(Vector3.zero, _selectionViewTransitionDuration).SetEase(_selectionViewDisappearAnimationCurve).OnComplete(() => _selectionViewGraphics.gameObject.SetActive(false));
    }
    
    private async UniTask EnableSelectionIndicator()
    {
        _currentSelectionIndicatorContainer = _selectionIndicatorContainers.Find(container => container.Type == _selectionManager.SelectionType);
        TextMeshPro indicator = _currentSelectionIndicatorContainer.Indicator;
        indicator.gameObject.SetActive(true);
        indicator.DOKill();
        await indicator.DOFade(1f, 1f).From(0f).AsyncWaitForCompletion();
    }
    
    private async UniTask DisableSelectionIndicator()
    {
        TextMeshPro indicator = _currentSelectionIndicatorContainer.Indicator;
        indicator.DOKill();
        await indicator.DOFade(0f, 1f).From(1f).AsyncWaitForCompletion();
        indicator.gameObject.SetActive(false);
    }
    
    [Serializable] private sealed class SelectionIndicatorContainers
    {
        [SerializeField] private SelectionType _selectionType;
        [SerializeField] private TextMeshPro _selectionIndicator;
        
        public SelectionType Type => _selectionType;
        public TextMeshPro Indicator => _selectionIndicator;
    }
}