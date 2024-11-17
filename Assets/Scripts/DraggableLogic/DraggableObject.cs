using UnityEngine;
using UnityEngine.Events;
using System;
using Cashing;

[RequireComponent(typeof(Collider))]

public class DraggableObject : MonoBehaviour, IDraggable
{  
    [Cached] private Collider _collider;
    
    [SerializeField] private PlacementCondition _placementRequirements;
    [SerializeField] private bool _isDraggable = true;
    private DraggableState _draggableState;

    public Action PickedUp;
    public Action Placed;
    public Action<DraggableObject> DraggablePickedUp;
    public Action<DraggableObject> DraggablePlaced;
    
    void IDraggable.PickUp()
    {
        _draggableState = DraggableState.Placed;
        _collider.isTrigger = true;

        PickedUp?.Invoke();
        DraggablePickedUp?.Invoke(this);
    }
    void IDraggable.Place() 
    { 
        _draggableState = DraggableState.Dragged;
        _collider.isTrigger = false;

        Placed?.Invoke();
        DraggablePlaced?.Invoke(this);
    }
    bool IDraggable.IsDraggable() => _isDraggable && _draggableState == DraggableState.Placed;
    PlacementCondition IDraggable.GetPlacementCondition() => _placementRequirements;
    
    public void SetDraggableState(bool state) => _isDraggable = state;
    public bool IsPlaced() => _draggableState == DraggableState.Placed;
}

public enum DraggableState
{
    Placed, 
    Dragged
}