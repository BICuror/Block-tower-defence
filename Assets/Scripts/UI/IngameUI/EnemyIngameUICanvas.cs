using Cashing;

public class EnemyIngameUICanvas : DynamicUIElement
{
    [Cached] private DraggableObject _draggableObject;

    private void Start()
    {
        _draggableObject.PickedUp += DisableCanvas;
        _draggableObject.Placed += EnableCanvas;
    }

    private void EnableCanvas() => gameObject.SetActive(true);
    private void DisableCanvas() => gameObject.SetActive(false);
}