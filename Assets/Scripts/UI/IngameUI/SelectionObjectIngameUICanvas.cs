using UnityEngine;

public sealed class SelectionObjectIngameUICanvas : StaticUIElement
{
    [SerializeField] private DraggableObject _draggable;

    private void Start()
    {
        base.Start();
        
        _draggable.PickedUp += DisableCanvas;
        _draggable.Placed += EnableCanvas;
    }

    private void EnableCanvas() => gameObject.SetActive(true);
    private void DisableCanvas() => gameObject.SetActive(false);
}