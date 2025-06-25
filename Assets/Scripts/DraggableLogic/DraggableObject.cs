using UnityEngine;
using UnityEngine.Events;
using System;
using Cashing;
using Combat;

[RequireComponent(typeof(Collider))]

public class DraggableObject : MonoBehaviour, IDraggable
{
    [SerializeField] private DragAnimationObject _dragAnimationObject;
    [SerializeField] private PlacementModule _placementRequirements;
    [SerializeField] private bool _isDraggable = true;
    private Collider _collider;
    
    protected DraggableState DraggableState;
    
    public bool IsPlaced => DraggableState == DraggableState.Placed;

    public Action PickedUp;
    public Action Placed;
    
    public Action<DraggableObject> DraggablePickedUp; 
    public Action<DraggableObject> DraggablePlaced;

    protected void Awake()
    {
        _collider = GetComponent<Collider>();    
    }
    
    void IDraggable.PickUp()
    {
        DraggableState = DraggableState.Dragged;
        _collider.isTrigger = true;

        PickedUp?.Invoke();
        DraggablePickedUp?.Invoke(this);
    }
    
    void IDraggable.Place() 
    { 
        DraggableState = DraggableState.Placed;
        _collider.isTrigger = false;

        Placed?.Invoke();
        DraggablePlaced?.Invoke(this);
    }
    
    public bool IsDraggable() => _isDraggable && DraggableState == DraggableState.Placed;
    
    DragAnimationObject IDraggable.GetDragAnimationObject() => _dragAnimationObject;
    
    public void SetDraggableState(bool state) => _isDraggable = state;
    
    public PlacementModule GetPlacementModule() => _placementRequirements; 
    public void SetNewDragAnimationObject(DragAnimationObject newAnimationObject) => _dragAnimationObject = newAnimationObject;
}

public enum DraggableState
{
    Placed, 
    Dragged
}