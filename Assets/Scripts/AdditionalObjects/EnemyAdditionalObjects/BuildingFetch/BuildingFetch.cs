using System.Collections.Generic;
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
        await MoveTo(_buildingEntity.transform.position);

        if (_currentState != FetchState.Chase) return;

        _currentState = FetchState.Dragging;

        _draggableConnector.PickUpDraggable(_buildingEntity.gameObject);

        Vector3 placementPosition = GetValidPlacementPosition(GetDesiredPlacementPosition());
        
        await MoveTo(placementPosition);

        if (_currentState != FetchState.Dragging) return;

        await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject,
            _buildingEntity.ComponentsContainer.Get<BuildingDraggable>(), placementPosition);

        _currentState = FetchState.Idle;
    }

    private async UniTask MoveTo(Vector3 position)
    {
        float duration = Vector3.Distance(position, _draggableConnector.transform.position) * _timePerTile;
        
        await _draggableConnector.transform.DOMove(position, duration).AsyncWaitForCompletion();
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
    
    private Vector3 GetValidPlacementPosition(Vector3 desiredPosition)
    {
        Vector2Int roundedDesiredPosition = new Vector2Int(Mathf.RoundToInt(desiredPosition.x), Mathf.RoundToInt(desiredPosition.z));

        List<Vector2Int> possiblePositions = TileMap.ForceGetSuitablePositionsInRadius(IsValidPosition, roundedDesiredPosition, 0);

        Vector2Int finalPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

        float height = _buildingEntity.ComponentsContainer.Get<BuildingDraggable>().GetPlacementModule().GetHeight(finalPosition);
        
        return new Vector3(finalPosition.x, height, finalPosition.y);
        
        bool IsValidPosition(Vector2Int position)
        {
            if (!_buildingEntity.ComponentsContainer.Get<BuildingDraggable>().GetPlacementModule().CanBePlaced(position)) return false;

            return !TileMap.HasTile(position, _roadLayerSetting);
        }
    }

    private async void OnOwnerDeath()
    {
        _ownerEntity.Health.Died -= OnOwnerDeath;

        if (_currentState == FetchState.Dragging)
        {
            Vector3 placementPosition = GetValidPlacementPosition(_draggableConnector.transform.position);
            
            await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject, _buildingEntity.ComponentsContainer.Get<BuildingDraggable>(), placementPosition);
        }

        _currentState = FetchState.Idle;
        _draggableConnector.DOKill();
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