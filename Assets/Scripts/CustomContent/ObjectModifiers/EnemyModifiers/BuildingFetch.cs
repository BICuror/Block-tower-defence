using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Cashing;
using Combat;

public sealed class BuildingFetch : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private LayerSetting _roadLayerSetting;
    [SerializeField] private BuildingAreaScaner _buildingAreaScaner;
    [SerializeField] private DraggableConnector _draggableConnector;
    [SerializeField] private float _timePerTile = 1f;
    [SerializeField] private float _fetchDistance = 3f;
    [SerializeField] private FetchType _fetchType;
    private BuildingEntity _buildingEntity;
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
            _draggableConnector.MoveTowardsPosition(_ownerEntity.transform.position);
    }

    private async void TryStartChase(BuildingEntity buildingEntity)
    {
        if (_currentState != FetchState.Idle) return;

        if (buildingEntity.ComponentsContainer.Get<BuildingDraggable>().IsDraggable())
        {
            _currentState = FetchState.Chase;

            _buildingEntity = buildingEntity;

            FetchBuilding();
        }
    }

    private async UniTask FetchBuilding()
    {
        await _draggableConnector.MoveTo(_buildingEntity.transform.position, _timePerTile);

        if (_currentState != FetchState.Chase || !_buildingEntity.Draggable.IsDraggable())
        {
            _currentState = FetchState.Idle;
            return;
        }

        _currentState = FetchState.Dragging;

        _draggableConnector.PickUpDraggable(_buildingEntity.gameObject);

        Vector3 placementPosition = TileMap.GetNearestPlacePosition(_buildingEntity.Draggable, GetDesiredPlacementPosition(), position => !TileMap.HasTile(position, _roadLayerSetting));
        
        await _draggableConnector.MoveTo(placementPosition, _timePerTile);

        if (_currentState != FetchState.Dragging) return;

        await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject, _buildingEntity.Draggable, placementPosition);

        _currentState = FetchState.Idle;
    }
    
    private Vector3 GetDesiredPlacementPosition()
    {
        switch (_fetchType)
        {
            case FetchType.From:
            {
                Vector3 direction = (_buildingEntity.transform.position - _ownerEntity.transform.position).normalized;

                return _buildingEntity.transform.position + direction * _fetchDistance;
            }
            case FetchType.To:
            {
                Vector3 direction = (_ownerEntity.transform.position - _buildingEntity.transform.position).normalized;

                return _buildingEntity.transform.position + direction * _fetchDistance;
            }
        }

        return Vector3.one;
    }

    private async void OnOwnerDeath()
    {
        _ownerEntity.Health.Died -= OnOwnerDeath;

        if (_currentState == FetchState.Dragging)
        {
            Vector3 placementPosition = TileMap.GetNearestPlacePosition(_buildingEntity.ComponentsContainer.Get<BuildingDraggable>(), transform.position, position => !TileMap.HasTile(position, _roadLayerSetting));
            
            await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject, _buildingEntity.ComponentsContainer.Get<BuildingDraggable>(), placementPosition);
        }

        _currentState = FetchState.Idle;
        _draggableConnector.DOKill();
        _buildingAreaScaner.AddedItem -= TryStartChase;
        Destroy(_draggableConnector.gameObject);
        Destroy(gameObject);
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