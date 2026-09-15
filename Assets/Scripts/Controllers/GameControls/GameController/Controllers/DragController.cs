using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace GameControls.Controllers
{
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
        [SerializeField] private Camera _camera;
        [SerializeField] private DraggableConnector _draggableConnector;
    
        private DraggableObject _currentDraggableObject;
        private IDraggable _currentIDraggable;
        
        private Vector3 _lastValidPlacementPosition;
        
        public event Action<DraggableObject> PickedObject;
        public event Action<DraggableObject> DroppedObject;
        
        public bool HoveredOverDraggableObject(Vector2 mousePosition, out GameObject draggableObject)
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
                _currentDraggableObject = rayInfo.collider.gameObject.GetComponent<DraggableObject>();
                _currentIDraggable = rayInfo.collider.gameObject.GetComponent<IDraggable>();
                _lastValidPlacementPosition = rayInfo.collider.transform.position;
                _draggableConnector.transform.position = rayInfo.collider.transform.position;
    
                _draggableConnector.PickUpDraggable(_currentDraggableObject.gameObject);
    
                if (CanBePlacedAt(GetPlacementPosition(_lastValidPlacementPosition)) == false)
                {
                    _lastValidPlacementPosition = FindSuitablePositionNearby(_lastValidPlacementPosition.x, _lastValidPlacementPosition.z);
                }
    
                PickedObject?.Invoke(_currentDraggableObject);
            }
        }
        
        public void TryDragTo(Vector2 mousePosition)
        {
            Ray ray = _camera.ScreenPointToRay(mousePosition);
    
            if (Physics.Raycast(ray, out RaycastHit rayInfo, Mathf.Infinity, _waterAndTerrainLayerSettings.GetLayerMask()))
            {
                Vector2 placementPosition = GetPlacementPosition(rayInfo.point);

                if (!CanBePlacedAt(placementPosition))
                {
                    placementPosition = GetPlacementPosition(TileMap.GetNearestDraggablePlacePosition(_currentDraggableObject, rayInfo.point, maxRadius: 50));
                }

                float height = GetPlacementHeight(placementPosition);
                    
                _lastValidPlacementPosition = new Vector3(placementPosition.x, height, placementPosition.y);
            }
            
            _currentIDraggable.OnDrag();
            MoveDraggable();
        }
    
        private void MoveDraggable() => _draggableConnector.MoveTowardsPosition(_lastValidPlacementPosition);
    
        public void DropDraggable(Vector2 mousePosition)
        {
            TryDragTo(mousePosition);
            
            DroppedObject?.Invoke(_currentDraggableObject);
    
            _draggableConnector.PlaceDraggable(_currentDraggableObject.gameObject, _currentIDraggable, _lastValidPlacementPosition).Forget();
    
            _currentIDraggable = null;
            _currentDraggableObject = null;
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
                            Vector2 placementPosition = GetPlacementPosition(new Vector3(x, 0, y));
                        
                            float height = GetPlacementHeight(position);
                            
                            return new Vector3(placementPosition.x, height, placementPosition.y);
                        }
                    } 
                }
            }
    
            return Vector3.zero;
        }
        
        private bool CanBePlacedAt(Vector2 position)
        {
            if (_currentIDraggable.GetPlacementModule())
            {
                return _currentIDraggable.GetPlacementModule().CanBePlaced(position, _currentIDraggable.TileScale);
            }
            
            return _defaultPlacementModule.CanBePlaced(position, _currentIDraggable.TileScale);
        }
    
        private float GetPlacementHeight(Vector2 position)
        {
            if (_currentIDraggable.GetPlacementModule())
            {
                return _currentIDraggable.GetPlacementModule().GetHeight(position);
            }
            
            return _defaultPlacementModule.GetHeight(position);      
        }
    
        private Vector2 GetPlacementPosition(Vector3 mousePosition)
        {
            Vector2Int roundedMousePosition = (new Vector2(mousePosition.x, mousePosition.z) - TileMap.GetTileSizeDraggableObjectOffset(_currentIDraggable.TileScale)).ToIntVector();
            
            PlacementModule placementModule = _defaultPlacementModule;
            
            if (_currentIDraggable.GetPlacementModule())
            {
                placementModule = _currentIDraggable.GetPlacementModule();
            }
            
            Vector2 placementPosition = placementModule.GetPlacementPosition(roundedMousePosition, _currentIDraggable.TileScale);
            
            return placementPosition + TileMap.GetTileSizeDraggableObjectOffset(_currentIDraggable.TileScale);
        }
    }
}
