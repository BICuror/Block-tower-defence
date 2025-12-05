using UnityEngine;
using Cashing;

public sealed class DraggableEntityCanvas : StaticUIElement
{
    [Cached] private BuildingDraggable _buildingDraggable;
    [SerializeField] private GameObject _mainCanvasParent;

    private void Start()
    {
        base.Start();
        
        _buildingDraggable.PickedUp += DisableCanvas;
        _buildingDraggable.BuildingBuilt += _ => EnableCanvas();
    }

    private void EnableCanvas() => _mainCanvasParent.SetActive(true);
    private void DisableCanvas() => _mainCanvasParent.SetActive(false);
}