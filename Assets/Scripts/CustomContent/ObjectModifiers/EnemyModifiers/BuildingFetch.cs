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
    [SerializeField] private Animator _animator;
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
        {
            _draggableConnector.MoveTowardsPosition(_ownerEntity.transform.position);
            
            _draggableConnector.transform.rotation = _ownerEntity.ComponentsContainer.Get<DragAnimationObject>().transform.rotation;
        }
    }

    private void TryStartChase(BuildingEntity buildingEntity)
    {
        if (_currentState != FetchState.Idle) return;

        if (buildingEntity.Draggable.IsDraggable())
        {
            SetState(FetchState.Chase);

            _buildingEntity = buildingEntity;
            _buildingEntity.Draggable.PickedUp += StopChase;

            FetchBuilding();
        }
    }

    private void StopChase()
    {
        _buildingEntity.Draggable.PickedUp -= StopChase;
        _buildingEntity = null;
        
        SetState(FetchState.Idle);
        _draggableConnector.StopCurrentMovement();
    }
    
    private async UniTask FetchBuilding()
    {
        await _draggableConnector.MoveToPerTile(_buildingEntity.transform.position, _timePerTile);

        if (_currentState == FetchState.Chase && _buildingEntity)
        {
            SetState(FetchState.Dragging);
            
            _buildingEntity.Draggable.PickedUp -= StopChase;
            _draggableConnector.PickUpDraggable(_buildingEntity.gameObject);
       
            Vector3 travelDestination = TileMap.GetNearestPlacePosition(_buildingEntity.Draggable, GetDesiredPlacementPosition(), position => !TileMap.HasTile(position, _roadLayerSetting));
               
            await _draggableConnector.MoveToPerTile(travelDestination, _timePerTile);
            
            Vector3 placementPosition = TileMap.GetNearestPlacePosition(_buildingEntity.Draggable, _draggableConnector.transform.position, position => !TileMap.HasTile(position, _roadLayerSetting));
       
            await _draggableConnector.PlaceDraggable(_buildingEntity.gameObject, _buildingEntity.Draggable, placementPosition);
        }
        
        SetState(FetchState.Idle);
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
        _buildingAreaScaner.AddedItem -= TryStartChase;
        
        Destroy(gameObject);

        await UniTask.WaitUntil(() => _currentState == FetchState.Idle);
        
        Destroy(_draggableConnector.gameObject);
    }

    private void SetState(FetchState state)
    {
        _currentState = state;
        
        //_animator.SetBool("IsOpen", _currentState != FetchState.Idle);
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