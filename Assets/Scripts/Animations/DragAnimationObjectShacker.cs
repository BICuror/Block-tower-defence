using NaughtyAttributes;
using DG.Tweening;
using UnityEngine;

namespace Combat.Animation
{
    [RequireComponent(typeof(DragAnimationObject))]
    
    public sealed class DragAnimationObjectShacker : Shaker
    {
        [SerializeField] private HoverableObject _hoverableObject;
    
        [Header("Scale")] 
        [SerializeField] private bool _useTileScaleInsteadOfFixedWhenPickedUp;
        [SerializeField] private float _pickedUpScale = 0.7f;
        [SerializeField] private bool _hasStaticYScale;
        [ShowIf("_hasStaticYScale")] [SerializeField] private float _defaultYScale;
        private Tween _scaleTween;
        
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
            if (_hasStaticYScale) SetDefaultValues(new Vector3(scale, _defaultYScale, scale));
            else SetDefaultValues(new Vector3(scale, scale, scale));
            _mesh.localScale = DefaultScale;
        }
        
        private void TryShake()
        {
            if (!_dragAnimationIsActive && !_dragAnimationObject.IsDragged) Shake();
        }
    
        private void OnDragAnimationStarted()
        {
            StopScaleTween();
    
            Vector3 pickedUpScale;
            
            if (_useTileScaleInsteadOfFixedWhenPickedUp) pickedUpScale = DefaultScale * _pickedUpScale;
            else pickedUpScale = new Vector3(_pickedUpScale, _pickedUpScale, _pickedUpScale);
            
            _scaleTween = _mesh.DOScale(pickedUpScale, 0.4f).SetLink(gameObject).OnComplete(() => _dragAnimationIsActive = true);
        }
    
        private void StopScaleTween()
        {
            if (_scaleTween != null && _scaleTween.IsActive()) _scaleTween.Kill();
        }
        
        private void OnDragAnimationEnded()
        {
            StopScaleTween();
            
            _scaleTween = _mesh.DOScale(DefaultScale, 0.4f).SetLink(gameObject).OnComplete(() => _dragAnimationIsActive = false);
        }
    }
}