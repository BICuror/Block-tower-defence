using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]

public sealed class DragController : MonoBehaviour
{
    [Header("LayerSettings")]
    [SerializeField] private LayerSetting _draggableObjectLayerSettings;
    [SerializeField] private LayerSetting _activatableObjectLayerSettings;
    [SerializeField] private LayerSetting _solidObjectsLayerSettings;

    [Header("PlacementLayers")]
    [SerializeField] private LayerSetting _terrainLayerSettings;
    [SerializeField] private LayerSetting _terrainAndRoadLayerSettings;
    [SerializeField] private LayerSetting _waterAndTerrainLayerSettings; 

    [Header("DragSettings")]
    [SerializeField] private float _placingHeight;
    [SerializeField] private float _dragSpeed;

    [Header("Links")]
    [SerializeField] private DraggableConnector _draggableConnector;

    private Camera _camera;

    public UnityEvent<GameObject> PickedObject;
    public UnityEvent<GameObject> DroppedObject;

    private GameObject _currentDraggableGameObject;
    private IDraggable _currentIDraggable;
    private Vector3 _lastValuablePosition;


    private void OnEnable() => _camera = GetComponent<Camera>();

    public void DropDraggable(Vector2 mousePosition)
    {
        TryDragTo(mousePosition);

        _draggableConnector.DisconnectDraggable(_currentDraggableGameObject.gameObject);

        _draggableConnector.StartPlacementAnimation(_currentDraggableGameObject, GetLastSnappedGridPosition());

        DroppedObject.Invoke(_currentDraggableGameObject);

        _currentIDraggable = null;

        _currentDraggableGameObject = null;
    }

    private Vector3 GetLastSnappedGridPosition()
    {
        LayerSetting currentTerrainLayerSetting = GetCurrentDraggableLayerSetting();
        
        Vector3 placePosition = new Vector3(Mathf.RoundToInt(_lastValuablePosition.x), 0, Mathf.RoundToInt(_lastValuablePosition.z)); 

        Ray heightRay = new Ray(new Vector3(placePosition.x, 100000f, placePosition.z), Vector3.down);

        if (Physics.Raycast(heightRay, out RaycastHit heightRayInfo, Mathf.Infinity, currentTerrainLayerSetting.GetLayerMask()))
        {
            return new Vector3(placePosition.x, heightRayInfo.point.y + _placingHeight, placePosition.z);
        }

        return Vector3.zero;
    }

    public void PickUpDraggable(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, _draggableObjectLayerSettings.GetLayerMask()))
        {
            _currentDraggableGameObject = rayInfo.collider.gameObject;
            _currentIDraggable = rayInfo.collider.gameObject.GetComponent<IDraggable>();
            _lastValuablePosition = rayInfo.collider.transform.position;
            _draggableConnector.transform.position = rayInfo.collider.transform.position;

            _currentIDraggable.PickUp();

            _draggableConnector.ConnectDraggable(_currentDraggableGameObject);

            PickedObject.Invoke(_currentDraggableGameObject);
        }
    }

    public LayerSetting GetCurrentDraggableLayerSetting() 
    {
        switch (_currentIDraggable.GetPlacementRequirements())
        {
            case PlacementRequirements.SolidSurface: return _terrainLayerSettings;
            case PlacementRequirements.TransitionSurface: return _terrainAndRoadLayerSettings;
            default: return _waterAndTerrainLayerSettings;
        }
    }

    public void TryDragTo(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        LayerSetting currentTerrainLayerSetting = GetCurrentDraggableLayerSetting();

        if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, currentTerrainLayerSetting.GetLayerMask()))
        {
            Vector3 roundedRayPosition = new Vector3(Mathf.RoundToInt(rayInfo.point.x), Mathf.RoundToInt(rayInfo.point.y), Mathf.RoundToInt(rayInfo.point.z));

            Ray heightRay = new Ray(new Vector3(roundedRayPosition.x, 100000f, roundedRayPosition.z), Vector3.down);

            if (Physics.Raycast(heightRay, out RaycastHit heightRayInfo, Mathf.Infinity, currentTerrainLayerSetting.GetLayerMask()))
            {
                if (CanBePlacedAt(roundedRayPosition.x, roundedRayPosition.z))
                {
                    _lastValuablePosition = new Vector3(roundedRayPosition.x, heightRayInfo.point.y + _placingHeight, roundedRayPosition.z);
                }
            }
        }

        MoveDraggable();
    }

    private void MoveDraggable()
    {
        float distance = Vector3.Distance(_lastValuablePosition, _draggableConnector.transform.position);

        _draggableConnector.transform.position = Vector3.MoveTowards(_draggableConnector.transform.position, _lastValuablePosition, _dragSpeed * distance);        
    }

    public bool CanBePlacedAt(float x, float z)
    {
        return _currentIDraggable.CanBePlacedAt(x, z, _solidObjectsLayerSettings);
    }

    public bool PickedUpDraggable(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _draggableObjectLayerSettings.GetLayerMask()))
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

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _activatableObjectLayerSettings.GetLayerMask()))
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
