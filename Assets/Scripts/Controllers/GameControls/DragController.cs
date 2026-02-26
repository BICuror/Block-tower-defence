using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public sealed class DragController : MonoBehaviour
{
    [Header("LayerSettings")]
    [SerializeField] private LayerSetting _draggableObjectLayerSettings;
    [SerializeField] private LayerSetting _activatableObjectLayerSettings;

    [Header("PlacementSettings")]
    [SerializeField] private LayerSetting _waterAndTerrainLayerSettings; 
    [SerializeField] private PlacementModule _defaultPlacementModule;

    [Header("DragSettings")]
    [SerializeField] private float _placingHeight;

    [Header("Links")]
    [SerializeField] private DraggableConnector _draggableConnector;

    private Camera _camera;

    public UnityEvent<GameObject> PickedObject;
    public UnityEvent<GameObject> DroppedObject;

    private GameObject _currentDraggableGameObject;
    private IDraggable _currentIDraggable;
    private Vector3 _lastValuablePosition;

    private void OnEnable() => _camera = GetComponent<Camera>();
    
    public bool PickedUpDraggable(Vector2 mousePosition, out GameObject draggableObject)
    {
        draggableObject = null;
        
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _draggableObjectLayerSettings.GetLayerMask()))
        {
            if (rayInfo.collider.gameObject.TryGetComponent(out IDraggable draggable))
            {
                draggableObject = rayInfo.collider.gameObject;
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

    public bool HoveredOverActivatable(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, 100000f, _activatableObjectLayerSettings.GetLayerMask()))
        {
            if (rayInfo.collider.gameObject.TryGetComponent(out IActivatable activatable))
            {
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

            _draggableConnector.PickUpDraggable(_currentDraggableGameObject);

            if (CanBePlacedAt(GetPlacmentPosition(_lastValuablePosition)) == false)
            {
                _lastValuablePosition = FindSuitablePositionNearby(_lastValuablePosition.x, _lastValuablePosition.z);
            }

            PickedObject.Invoke(_currentDraggableGameObject);
        }
    }
    
    public void TryDragTo(Vector2 mousePosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, _waterAndTerrainLayerSettings.GetLayerMask()))
        {
            Vector2Int placementPosition = GetPlacmentPosition(rayInfo.point);
            
            if (CanBePlacedAt(placementPosition))
            {
                float height = GetPlacementHeight(placementPosition);
                
                _lastValuablePosition = new Vector3(placementPosition.x, height, placementPosition.y);
            }
        }
        
        _currentIDraggable.OnDrag();
        MoveDraggable();
    }

    private void MoveDraggable() => _draggableConnector.MoveTowardsPosition(_lastValuablePosition);

    public void DropDraggable(Vector2 mousePosition)
    {
        TryDragTo(mousePosition);
        
        DroppedObject.Invoke(_currentDraggableGameObject);

        _draggableConnector.PlaceDraggable(_currentDraggableGameObject, _currentIDraggable, GetLastSnappedGridPosition());

        _currentIDraggable = null;
        _currentDraggableGameObject = null;
    }

    private Vector3 GetLastSnappedGridPosition()
    {
        Vector2Int placementPosition = GetPlacmentPosition(_lastValuablePosition);
        
        Vector3 placePosition = new Vector3(placementPosition.x, 0, placementPosition.y); 

        float height = GetPlacementHeight(placementPosition);
    
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
                    Vector2Int position = new Vector2Int(searchX + x, searchY + y);
                    
                    if (CanBePlacedAt(position))
                    {
                        float height = GetPlacementHeight(position);
                    
                        return new Vector3(searchX + x, height, searchY + y);
                    }
                } 
            }
        }

        return Vector3.zero;
    }
    
    private bool CanBePlacedAt(Vector2Int position)
    {
        if (_currentIDraggable.GetPlacementModule())
        {
            return _currentIDraggable.GetPlacementModule().CanBePlaced(position);
        }
        
        return _defaultPlacementModule.CanBePlaced(position);
    }

    private float GetPlacementHeight(Vector2Int position)
    {
        if (_currentIDraggable.GetPlacementModule())
        {
            return _currentIDraggable.GetPlacementModule().GetHeight(position);
        }
        
        return _defaultPlacementModule.GetHeight(position);      
    }

    private Vector2Int GetPlacmentPosition(Vector3 mousePosition)
    {
        Vector2Int position = new Vector2Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.z));
        
        if (_currentIDraggable.GetPlacementModule())
        {
            return _currentIDraggable.GetPlacementModule().GetPlacementPosition(position);
        }
        
        return _defaultPlacementModule.GetPlacementPosition(position);     
    }
}