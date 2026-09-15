using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(DragAnimationObject))]

public sealed class DragAnimationObjectShacker : Shaker
{
    [SerializeField] private HoverableObject _hoverableObject;
    private DragAnimationObject _dragAnimationObject;
    private bool _dragAnimationIsActive;
    
    private void Awake()
    {
        Initialize();
        
        _hoverableObject.HoverEntered.AddListener(TryShake);
        
        _dragAnimationObject = GetComponent<DragAnimationObject>();

        _dragAnimationObject.DragStarted += OnDragAnimationStarted;
        _dragAnimationObject.DragEnded += OnDragAnimationEnded;
        
        _dragAnimationObject.DefaultScaleChanged += SetDefaultScale;
    }

    private void SetDefaultScale(float scale)
    {
        SetDefaultValues(new Vector3(scale, scale, scale));
        _mesh.localScale = DefaultScale;
    }
    
    private void TryShake()
    {
        if (!_dragAnimationIsActive) Shake();
    }

    private void OnDragAnimationStarted()
    {
        _mesh.DOKill();
        
        _mesh.DOScale(Vector3.one * 0.7f, 0.4f).SetLink(gameObject).OnComplete(() => _dragAnimationIsActive = true);
    }
    
    private void OnDragAnimationEnded()
    {
        _mesh.DOKill();
        
        _mesh.DOScale(DefaultScale, 0.4f).SetLink(gameObject).OnComplete(() => _dragAnimationIsActive = false);
    }
}