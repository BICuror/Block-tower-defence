using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class DragController : MonoBehaviour
{
    private const float ADDITIONAL_PLACEMENT_HEIGHT = 0.5f;
    
    [Header("LayerSettings")]
    [SerializeField] private LayerSetting _draggableObjectLayerSettings;
    [SerializeField] private LayerSetting _activatableObjectLayerSettings;

    [Header("PlacementSettings")]
    [SerializeField] private LayerSetting _waterAndTerrainLayerSettings; 
    [SerializeField] private PlacementModule _defaultPlacementModule;

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

            if (CanBePlacedAt(_lastValuablePosition.x, _lastValuablePosition.z) == false) _lastValuablePosition = FindSuitablePositionNearby(_lastValuablePosition.x, _lastValuablePosition.z);

            PickedObject.Invoke(_currentDraggableGameObject);
        }
    }
    
    public void TryDragTo(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, _waterAndTerrainLayerSettings.GetLayerMask()))
        {
            Vector3 roundedRayPosition = new Vector3(Mathf.RoundToInt(rayInfo.point.x), Mathf.RoundToInt(rayInfo.point.y), Mathf.RoundToInt(rayInfo.point.z));
            
            Vector2Int placementPosition = GetPlacmentPosition(roundedRayPosition.x, roundedRayPosition.z);

            if (CanBePlacedAt(placementPosition.x, placementPosition.y))
            {
                float height = GetPlacementHeight(placementPosition.x, placementPosition.y);
                
                _lastValuablePosition = new Vector3(placementPosition.x, height, placementPosition.y);
            }
        }

        MoveDraggable();
    }

    private void MoveDraggable()
    {
        float distance = Vector3.Distance(_lastValuablePosition, _draggableConnector.transform.position);

        _draggableConnector.transform.position = Vector3.MoveTowards(_draggableConnector.transform.position, _lastValuablePosition, _dragSpeed * distance);        
    }

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
        Vector2Int placementPosition = GetPlacmentPosition(_lastValuablePosition.x, _lastValuablePosition.z);
        
        Vector3 placePosition = new Vector3(placementPosition.x, 0, placementPosition.y); 

        float height = GetPlacementHeight(placePosition.x, placePosition.z);
    
        return new Vector3(placePosition.x, height, placePosition.z);    
    }
    
    private Vector3 FindSuitablePositionNearby(float centerX, float centerZ)
    {
        int searchX = Mathf.RoundToInt(centerX);
        int searchY = Mathf.RoundToInt(centerZ);

        for (int radius = 1; radius <= 10; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (CanBePlacedAt(searchX + x, searchY + y))
                    {
                        Ray heightRay = new Ray(new Vector3(searchX + x, 100000f, searchY + y), Vector3.down);

                        float height = _placingHeight;

                        if (Physics.Raycast(heightRay, out RaycastHit heightRayInfo, Mathf.Infinity, _waterAndTerrainLayerSettings.GetLayerMask()))
                        {
                            if (heightRayInfo.point.y < 1) height += 1;
                            else height += heightRayInfo.point.y;        
                        }
                    
                        return new Vector3(searchX + x, height, searchY + y);
                    }
                } 
            }
        }

        return Vector3.zero;
    }
    
    private bool CanBePlacedAt(float x, float z)
    {
        int roundedX = Mathf.RoundToInt(x);
        int roundedZ = Mathf.RoundToInt(z);
        
        if (_currentIDraggable.GetPlacementModule())
        {
            return _currentIDraggable.GetPlacementModule().CanBePlaced(_currentDraggableGameObject, roundedX, roundedZ);
        }
        
        return _defaultPlacementModule.CanBePlaced(_currentDraggableGameObject, roundedX, roundedZ);
    }

    private float GetPlacementHeight(float x, float z)
    {
        int roundedX = Mathf.RoundToInt(x);
        int roundedZ = Mathf.RoundToInt(z);
        
        if (_currentIDraggable.GetPlacementModule())
        {
            return _currentIDraggable.GetPlacementModule().GetHeight(roundedX, roundedZ) + ADDITIONAL_PLACEMENT_HEIGHT;
        }
        
        return _defaultPlacementModule.GetHeight(roundedX, roundedZ) + ADDITIONAL_PLACEMENT_HEIGHT;      
    }

    private Vector2Int GetPlacmentPosition(float x, float z)
    {
        int roundedX = Mathf.RoundToInt(x);
        int roundedZ = Mathf.RoundToInt(z);
        
        if (_currentIDraggable.GetPlacementModule())
        {
            return _currentIDraggable.GetPlacementModule().GetPlacementPosition(roundedX, roundedZ);
        }
        
        return _defaultPlacementModule.GetPlacementPosition(roundedX, roundedZ);     
    }
}