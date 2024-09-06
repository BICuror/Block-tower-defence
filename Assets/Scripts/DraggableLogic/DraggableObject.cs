using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]

public class DraggableObject : MonoBehaviour, IDraggable
{  
    [SerializeField] private PlacementCondition _placementRequirements;
    [SerializeField] private bool _isDraggable = true;
    private bool _isDragged;

    private bool _isPlaced;

    private Collider _collider;

    public UnityEvent PickedUp;
    public UnityEvent Placed;

    [HideInInspector] public UnityEvent<DraggableObject> DraggablePickedUp;
    [HideInInspector] public UnityEvent<DraggableObject> DraggablePlaced;

    private void Awake() => _collider = GetComponent<Collider>();
    public void SetDraggableState(bool state) => _isDraggable = state;
    public bool IsDraggable() => _isDraggable && (_isDragged == false);
    public bool IsPlaced() => _isPlaced;
    public PlacementCondition GetPlacementCondition() => _placementRequirements;
    
    public void PickUp()
    {
        _isPlaced = false;

        _isDragged = true;

        _collider.isTrigger = true;

        PickedUp?.Invoke();

        DraggablePickedUp?.Invoke(this);
    }

    public void Place() 
    { 
        _isPlaced = true;

        _isDragged = false;

        _collider.isTrigger = false;

        Placed?.Invoke();

        DraggablePlaced?.Invoke(this);
    }
}