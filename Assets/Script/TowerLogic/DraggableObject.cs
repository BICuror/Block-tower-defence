using UnityEngine;
using UnityEngine.Events;

public class DraggableObject : MonoBehaviour, IDraggable
{   
    [SerializeField] private bool _isDraggable = true;

    private bool _isPlaced;

    public UnityEvent PickedUp;
    public UnityEvent Placed;

    public void SetDraggableState(bool state) => _isDraggable = state;
    public bool IsDraggable() => _isDraggable;
    
    public void PickUp()
    {
        _isPlaced = false;

        PickedUp?.Invoke();
    }

    public void Place() 
    { 
        _isPlaced = true;

        Placed?.Invoke();
    }

    public bool IsPlaced() => _isPlaced;
}
