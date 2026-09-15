using GameControls.Controllers;
using UnityEngine;

public sealed class DragControllerVisualiser : MonoBehaviour
{
    [SerializeField] private AreaVisualisationInspector _areaVisualisationInspector;
    [SerializeField] private SquareAreaVisualisation _placementArea;
    [SerializeField] private DragController _dragController;

    private void Awake()
    {
        _dragController.PickedObject += OnDragStarted;
        _dragController.DroppedObject += OnDragEnded;
    }

    private void OnDragStarted(DraggableObject draggedObject)
    {
        _areaVisualisationInspector.ActivateVisualisation(draggedObject.gameObject);
        _placementArea.SetDefaultScale(draggedObject.GetComponent<DraggableObject>().TileScale);
        _placementArea.EnableVisualisation();
    }

    private void OnDragEnded(DraggableObject draggedObject)
    {
        _areaVisualisationInspector.DeactivateVisualisation(draggedObject.gameObject);
        _placementArea.DisableVisualisation();
    }

    private void OnDestroy()
    {
        _dragController.PickedObject -= OnDragStarted;
        _dragController.DroppedObject -= OnDragEnded;
    }
}