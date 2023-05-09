using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]

public sealed class DragController : MonoBehaviour
{
    [Header("LayerSettings")]
    [SerializeField] private LayerMask _pickupObjectsLayers;
    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private LayerMask _dragPlainLayers;
   
    [Header("DragSettings")]
    [SerializeField] private float _placingHeight;
    [SerializeField] private float _dragSpeed;
    private Camera _camera;

    public UnityEvent<GameObject> PickedObject;
    public UnityEvent<GameObject> PlacedObject;

    private GameObject _draggableGameObject;
    private IDraggable _currentIDraggable;
    private Vector3 _lastValuablePosition;

    private void OnEnable() => _camera = GetComponent<Camera>();

    public void DropDraggable()
    {
        _currentIDraggable.Place();

        SnapDraggableToGrid();

        PlacedObject?.Invoke(_draggableGameObject);

        _currentIDraggable = null;
        _draggableGameObject = null;
    }

    private void SnapDraggableToGrid()
    {
        Vector3 placePosition = new Vector3(Mathf.RoundToInt(_lastValuablePosition.x), 0, Mathf.RoundToInt(_lastValuablePosition.z)); 

        Ray heightRay = new Ray(new Vector3(placePosition.x, 10000f, placePosition.z), Vector3.down);

        if (Physics.Raycast(heightRay, out RaycastHit heightRayInfo, Mathf.Infinity, _dragPlainLayers))
        {
            _draggableGameObject.transform.position = new Vector3(placePosition.x, heightRayInfo.point.y + _placingHeight, placePosition.z);
        }
    }

    public void PickUpDraggable(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, _pickupObjectsLayers))
        {
            _draggableGameObject = rayInfo.collider.gameObject;
            _currentIDraggable = rayInfo.collider.gameObject.GetComponent<IDraggable>();
            _lastValuablePosition = rayInfo.collider.transform.position;

            _currentIDraggable.PickUp();

            PickedObject?.Invoke(_draggableGameObject);
        }
    }

    public void TryDragTo(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, _dragPlainLayers))
        {
            Vector3 roundedRayPosition = new Vector3(Mathf.RoundToInt(rayInfo.point.x), Mathf.RoundToInt(rayInfo.point.y), Mathf.RoundToInt(rayInfo.point.z));

            Ray heightRay = new Ray(new Vector3(roundedRayPosition.x, 10000f, roundedRayPosition.z), Vector3.down);

            if (Physics.Raycast(heightRay, out RaycastHit heightRayInfo, Mathf.Infinity, _dragPlainLayers))
            {
                if (HasBuildingAt(roundedRayPosition.x, roundedRayPosition.z) == false)
                {
                    _lastValuablePosition = new Vector3(roundedRayPosition.x, heightRayInfo.point.y + _placingHeight, roundedRayPosition.z);

                    MoveDraggable();
                }
                else 
                {
                    MoveDraggable();
                }
            }
            else 
            {
                MoveDraggable();
            }
        }
        else 
        {
            MoveDraggable();
        }
    }

    private void MoveDraggable()
    {
        float distance = Vector3.Distance(_lastValuablePosition, _draggableGameObject.transform.position);

        _draggableGameObject.transform.position = Vector3.MoveTowards(_draggableGameObject.transform.position, _lastValuablePosition, _dragSpeed * distance);        
    }

    public bool HasBuildingAt(float x, float z)
    {
        Ray heightRay = new Ray(new Vector3(x, 10000f, z), Vector3.down);

        return Physics.Raycast(heightRay, out RaycastHit heightRayInfo, Mathf.Infinity, _buildingLayer) && heightRayInfo.collider.gameObject != _draggableGameObject;
    }

    public bool PickedUpDraggable(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 10000f, _pickupObjectsLayers))
        {
            if (rayInfo.collider.gameObject.TryGetComponent(out IDraggable draggable))
            {
                return draggable.IsDraggable();
            }
        }

        return false;
    }

    public bool ActivatedSomething(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 10000f, _pickupObjectsLayers))
        {
            if (rayInfo.collider.gameObject.TryGetComponent(out IActivatable activatable))
            {
                activatable.Activate();

                return true;
            }
        }

        return false;
    }
}
