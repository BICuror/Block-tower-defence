using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class BuildingFetch : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private LayerSetting _solidObjectsLayerSetting;
    [SerializeField] private LayerSetting _roadLayerSetting;
    [SerializeField] private AreaEntityDetector _buildingAreaScaner;
    [SerializeField] private DraggableConnector _draggableConnector;
    [SerializeField] private float _chasingTimePerTile = 0.65f;
    [SerializeField] private float _draggingTimePerTile = 0.2f;
    [SerializeField] private float _fetchDistance = 3f;
    [SerializeField] private FetchType _fetchType;
    private CombatEntity _currentTargetEntity;
    private FetchState _currentState;

    private void Start()
    {
        _ownerEntity.Health.Died += OnOwnerDeath;
        _buildingAreaScaner.AddedItem += TryStartChase;
        
        _draggableConnector.transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        if (_currentState == FetchState.Idle)
        {
            _draggableConnector.MoveTowardsPosition(_ownerEntity.transform.position);
            
            _draggableConnector.transform.rotation = _ownerEntity.ComponentsContainer.Get<DragAnimationObject>().transform.rotation;
        }
    }

    private void TryStartChase(CombatEntity entity)
    {
        if (_currentState != FetchState.Idle) return;

        if (entity.Draggable.IsDraggable())
        {
            SetState(FetchState.Chase);

            _currentTargetEntity = entity;
            _currentTargetEntity.Draggable.PickedUp += StopChase;

            FetchEntity().Forget();
        }
    }

    private void StopChase()
    {
        if (_currentState != FetchState.Chase) return;
        
        _currentTargetEntity.Draggable.PickedUp -= StopChase;
        _currentTargetEntity = null;
        
        SetState(FetchState.Idle);
        _draggableConnector.StopCurrentMovement();
    }
    
    private async UniTask FetchEntity()
    {
        await _draggableConnector.MoveToPerTile(_currentTargetEntity.transform.position, _chasingTimePerTile);

        if (_currentState == FetchState.Chase && _currentTargetEntity)
        {
            SetState(FetchState.Dragging);
            
            _currentTargetEntity.Draggable.PickedUp -= StopChase;
            _draggableConnector.PickUpDraggable(_currentTargetEntity.gameObject);
       
            Vector3 travelDestination = TileMap.GetNearestDraggablePlacePosition(_currentTargetEntity.Draggable, GetDesiredPlacementPosition(), IsValidPlacementPosition);
               
            await _draggableConnector.MoveToPerTile(travelDestination, _draggingTimePerTile);
            
            Vector2Int roundedDestanationPosition = new Vector2Int(Mathf.RoundToInt(travelDestination.x), Mathf.RoundToInt(travelDestination.z));

            Vector3 placementPosition = travelDestination;

            if (TileMap.GetTileCount(roundedDestanationPosition, _solidObjectsLayerSetting) > 2)
            {
                placementPosition = TileMap.GetNearestDraggablePlacePosition(_currentTargetEntity.Draggable, travelDestination, IsValidPlacementPosition);
            }
       
            await _draggableConnector.PlaceDraggable(_currentTargetEntity.gameObject, _currentTargetEntity.Draggable, placementPosition);
        }
        
        SetState(FetchState.Idle);
        
        return;

        bool IsValidPlacementPosition(Vector2Int position) => !TileMap.HasTile(position, _roadLayerSetting);
    }
    
    private Vector3 GetDesiredPlacementPosition()
    {
        switch (_fetchType)
        {
            case FetchType.From:
            {
                Vector3 direction = (_currentTargetEntity.transform.position - _ownerEntity.transform.position).normalized;

                return _currentTargetEntity.transform.position + direction * _fetchDistance;
            }
            case FetchType.To:
            {
                Vector3 direction = (_ownerEntity.transform.position - _currentTargetEntity.transform.position).normalized;

                return _currentTargetEntity.transform.position + direction * _fetchDistance;
            }
        }

        return Vector3.one;
    }

    private async void OnOwnerDeath()
    {
        _ownerEntity.Health.Died -= OnOwnerDeath;
        _buildingAreaScaner.AddedItem -= TryStartChase;
        
        Destroy(gameObject);

        await UniTask.WaitUntil(() => _currentState == FetchState.Idle);
        
        Destroy(_draggableConnector.gameObject);
    }

    private void SetState(FetchState state)
    {
        _currentState = state;
    }

    private enum FetchState
    {
        Idle,
        Chase,
        Dragging
    }

    private enum FetchType
    {
        To,
        From
    }
}