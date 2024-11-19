using UnityEngine;
using UnityEngine.Events;
using System;
using Cashing;
using Combat;

[RequireComponent(typeof(Collider))]

public class DraggableObject : MonoBehaviour, IDraggable
{  
    [SerializeField] private Collider _collider;
    [SerializeField] private PlacementCondition _placementRequirements;
    [SerializeField] private bool _isDraggable = true;
    private DraggableState _draggableState;

    public Action PickedUp;
    public Action Placed;
    
    void IDraggable.PickUp()
    {
        _draggableState = DraggableState.Dragged;
        _collider.isTrigger = true;

        PickedUp?.Invoke();
    }
    void IDraggable.Place() 
    { 
        _draggableState = DraggableState.Placed;
        _collider.isTrigger = false;

        Placed?.Invoke();
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