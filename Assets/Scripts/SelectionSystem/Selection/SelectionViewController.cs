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
    [SerializeField] private AnimationCurve _selectionViewAnimationCurve;
    
    [SerializeField] public SelectionManager _selectionManager;

    private void Awake()
    {
        _selectionManager.SelectionStarted += EnableSelectionView;
        _selectionManager.SelectionEnded += DisableSelectionView;
        
        _selectionManager.SelectionStepStarted += () => EnableSelectionIndicator().Forget();
        _selectionManager.SelectionStepEnded += () => DisableSelectionIndicator().Forget();
    }

    private void EnableSelectionView()
    {
        EnableSelectionIndicator().Forget();

        _selectionViewGraphics.DOKill();
        _selectionViewGraphics.DOScale(Vector3.one, _selectionViewTransitionDuration).SetEase(_selectionViewAnimationCurve);
        _selectionViewGraphics.gameObject.SetActive(true);
        
        //_postProcessingController.ChangeMainVolumePostExposure(-_postExposureDimStrength, _selectionViewTransitionDuration).Forget();
    }
    
    private void DisableSelectionView()
    {
        DisableSelectionIndicator().Forget();
        
        _selectionViewGraphics.DOKill();
        _selectionViewGraphics.DOScale(Vector3.zero, _selectionViewTransitionDuration).SetEase(_selectionViewAnimationCurve).OnComplete(() => _selectionViewGraphics.gameObject.SetActive(false));
        
        //_postProcessingController.ChangeMainVolumePostExposure(_postExposureDimStrength, _selectionViewTransitionDuration).Forget();
    }
    
    private async UniTask EnableSelectionIndicator()
    {
        TextMeshPro indicator = _selectionIndicatorContainers.Find(container => container.Type == _selectionManager.SelectionType).Indicator;
        indicator.gameObject.SetActive(true);
        indicator.DOKill();
        await indicator.DOFade(1f, 1f).From(0f).AsyncWaitForCompletion();
    }
    
    private async UniTask DisableSelectionIndicator()
    {
        TextMeshPro indicator = _selectionIndicatorContainers.Find(container => container.Type == _selectionManager.SelectionType).Indicator;
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