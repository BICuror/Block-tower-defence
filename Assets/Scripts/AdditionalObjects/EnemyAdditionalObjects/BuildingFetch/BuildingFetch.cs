using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Cashing;
using Combat;

public sealed class BuildingFetch : MonoBehaviour
{
    [Cached] private CombatEntity _ownerEntity;
    [SerializeField] private BuildingAreaScaner _buildingAreaScaner;
    [SerializeField] private float _timePerTile = 1f;
    [SerializeField] private DraggableConnector _draggableConnector;
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

        Vector3 newPosition = _buildingEntity.transform.position - _ownerEntity.transform.position +
                              _buildingEntity.transform.position;

        Vector3Int roundedPosition = Vector3Int.RoundToInt(newPosition);

        Vector2Int roundedPosition2D =
            new Vector2Int(Mathf.RoundToInt(roundedPosition.x), Mathf.RoundToInt(roundedPosition.z));

        List<Vector2Int> possiblePositions = TileMap.ForceGetSuitablePositionsInRadius(
            _buildingEntity.ComponentsContainer.Get<BuildingDraggable>().GetPlacementModule().CanBePlaced,
            roundedPosition2D, 0);

        roundedPosition2D = possiblePositions[Random.Range(0, possiblePositions.Count)];

        Vector3Int roundedPlacePosition = new Vector3Int(roundedPosition2D.x,
            (int)_buildingEntity.ComponentsContainer.Get<BuildingDraggable>().GetPlacementModule()
                .GetHeight(roundedPosition2D), roundedPosition2D.y);

        await MoveTo(roundedPlacePosition);

        if (_currentState != FetchState.Dragging) return;

        await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject,
            _buildingEntity.ComponentsContainer.Get<BuildingDraggable>(), roundedPlacePosition);

        _currentState = FetchState.Idle;
    }

    private async UniTask MoveTo(Vector3 position)
    {
        await _draggableConnector.transform
            .DOMove(position, Vector3.Distance(position, _draggableConnector.transform.position) * _timePerTile)
            .AsyncWaitForCompletion();
    }

    private async void OnOwnerDeath()
    {
        _ownerEntity.Health.Died -= OnOwnerDeath;

        if (_currentState == FetchState.Dragging)
        {
            Vector3Int roundedPosition = Vector3Int.RoundToInt(_draggableConnector.transform.position);

            Vector2Int roundedPosition2D =
                new Vector2Int(Mathf.RoundToInt(roundedPosition.x), Mathf.RoundToInt(roundedPosition.z));

            List<Vector2Int> possiblePositions = TileMap.ForceGetSuitablePositionsInRadius(
                _buildingEntity.ComponentsContainer.Get<BuildingDraggable>().GetPlacementModule().CanBePlaced,
                roundedPosition2D, 0);

            roundedPosition2D = possiblePositions[Random.Range(0, possiblePositions.Count)];

            Vector3Int roundedPlacePosition = new Vector3Int(roundedPosition2D.x,
                (int)_buildingEntity.ComponentsContainer.Get<BuildingDraggable>().GetPlacementModule()
                    .GetHeight(roundedPosition2D), roundedPosition2D.y);

            await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject,
                _buildingEntity.ComponentsContainer.Get<BuildingDraggable>(), roundedPlacePosition);
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
}