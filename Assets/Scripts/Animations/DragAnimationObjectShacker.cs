using UnityEngine;

[RequireComponent(typeof(DragAnimationObject))]

public sealed class DragAnimationObjectShacker : Shaker
{
    [SerializeField] private HoverableObject _hoverableObject;
    private DragAnimationObject _dragAnimationObject;
    
    private void Awake()
    {
        Initialize();
        
        _hoverableObject.HoverEntered.AddListener(TryShake);
        
        _dragAnimationObject = GetComponent<DragAnimationObject>();
    }
    
    private void TryShake()
    {
        if (!_dragAnimationObject.IsConnected) Shake();
    }
}