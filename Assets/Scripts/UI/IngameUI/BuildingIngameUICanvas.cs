using Cashing;

public sealed class BuildingIngameUICanvas : StaticUIElement
{
    [Cached] private BuildingDraggable _buildingDraggable;

    private void Start()
    {
        base.Start();
        
        _buildingDraggable.PickedUp += DisableCanvas;
        _buildingDraggable.Placed += EnableCanvas;
    }

    private void EnableCanvas() => gameObject.SetActive(true);
    private void DisableCanvas() => gameObject.SetActive(false);
}