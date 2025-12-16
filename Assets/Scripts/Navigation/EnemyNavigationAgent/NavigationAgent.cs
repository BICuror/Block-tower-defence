using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using System;

using Random = UnityEngine.Random;

namespace Navigation
{
    public sealed class NavigationAgent : MonoBehaviour
    {
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [SerializeField] private Transform _rotationTarget;
        [Cached] private DraggableEntity _draggableEntity;
        [Cached] private EntityHealth _entityHealth;
        [Cached] private Speed _speed;
        
        private CancellationTokenSource _movementCancellationTokenSource = new();
        private NavigationAgentData _agentData;
        private MovementNavigationModule _movementModule;
        private RotationNavigationModule _rotationModule;
        private NavigationAgentNodePicker _navigationAgentNodePicker;
        private NavigationMapLayer _currentNavigationMapLayer;
        private NavigationAgentNodePicker.WeightPickType _weightPickType;
        
        private NavigationNode _startNode;
        private NavigationNode _endNode;
        private NavigationNode _nextNode;
        private float _movementProgress;
        private bool _isEnabled;

        private List<Vector2Int> _checkDirection = new List<Vector2Int>()
        {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up
        };
        
        private void Start()
        {
            _draggableEntity.PickedUp += StopMovement;
            _draggableEntity.Placed += Initialize;
        }

        public void Disable()
        {
            _isEnabled = false;
            StopMovement();
        }

        public void Enable()
        {
            if (!_entityHealth.IsAlive()) return;
            
            _isEnabled = true;
            Initialize();
        }

        public void SetAgentData(NavigationAgentData agentData)
        {
            _agentData = agentData;
            _weightPickType = NavigationAgentNodePicker.WeightPickType.Minimal;
            SetNavigationAgentNodePicker(Type.GetType(agentData.NavgationNodePickerType));
            _movementProgress = 0f;
        }
        
        public void SetNavigationAgentNodePicker(Type navigationAgentNodePickerType)
        {
            _navigationAgentNodePicker = (NavigationAgentNodePicker)Activator.CreateInstance(navigationAgentNodePickerType);
            SetWeightPickLogic(_weightPickType);
        }

        public void SetWeightPickLogic(NavigationAgentNodePicker.WeightPickType weightPickType)
        {
            NavigationAgentNodePicker.WeightPickType initialWeightPickType = _weightPickType;
            
            _weightPickType = weightPickType;
            _navigationAgentNodePicker.SetWeightPickLogic(weightPickType);
            
            if (_weightPickType != initialWeightPickType)
            {
                _movementProgress = 1f - _movementProgress;

                (_startNode, _endNode) = (_endNode, _startNode);
                _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);
            }
            
            StopMovement();
            TravelToEndNode().Forget();
        }
        
        private void Initialize()
        {
            if (!_entityHealth.IsAlive()) return;
            
            _navigationMapHolder = NavigationMapHolder.Instance;
            
            StopMovement();
            FindSuitableLayer();
            AdaptToNavigationLayer();
            
            Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 nextPosition = new Vector2(_endNode.Position.x, _endNode.Position.z);

            Vector2Int previousRotation = new Vector2Int(-Mathf.RoundToInt(currentPosition.x - nextPosition.x), -Mathf.RoundToInt(currentPosition.y - nextPosition.y));
            
            _movementModule = new MovementNavigationModule(transform, _agentData);
            _rotationModule = new RotationNavigationModule(_rotationTarget, previousRotation);

            _isEnabled = true;
            TravelToEndNode().Forget();
        }
        
        private void AdaptToNavigationLayer()
        {
            Vector2Int currentRoundedPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

            _startNode = new NavigationNode(currentRoundedPosition, Mathf.RoundToInt(transform.position.y));
            _endNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, currentRoundedPosition);
            _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);
        }
        
        private async UniTask TravelToEndNode()
        {
            _movementModule.SetDestanation(new Vector3(transform.position.x, _startNode.Position.y, transform.position.z), _endNode.Position); 
            _rotationModule.SetPositions(new Vector3(transform.position.x, _startNode.Position.y, transform.position.z), _nextNode.Position);
            
            float duration = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), _endNode.RoundedPosition) * _speed.Value;
            float elapsedTime = duration * _movementProgress;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.fixedDeltaTime;

                _movementProgress = elapsedTime / duration;

                _movementModule.MoveTowardsNextPosition(_movementProgress);
                _rotationModule.RotateTowardsNode(_movementProgress);

                try
                {
                    await UniTask.WaitForFixedUpdate(_movementCancellationTokenSource.Token);

                    if (!_isEnabled) return;
                }
                catch (Exception e)
                {
                    e.LogAsync();
                    return;
                }
            }

            if (!_currentNavigationMapLayer.IsEnabled) FindSuitableLayer();

            IterateToNextNode();
            TravelToEndNode().Forget();
        }
        
        private void IterateToNextNode()
        {
            _startNode = _endNode;
            _endNode = _nextNode;   
            _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);

            _movementProgress = 0f;
        }

        private void StopMovement()
        {
            _movementCancellationTokenSource.Cancel();
            _movementCancellationTokenSource = new();
        }
        
        private void FindSuitableLayer()
        { 
            Vector2Int currentRoundedPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

            if (!_navigationMapHolder.Map.NodeExists(currentRoundedPosition))
            {
                List<Vector2Int> suitablePositions = TileMap.ForceGetSuitablePositionsInRadius(PositionValidator, currentRoundedPosition, 1);
                
                currentRoundedPosition = suitablePositions[Random.Range(0, suitablePositions.Count)];
            }
                
            if (_agentData.PrefferedNavigationLayer == NavigationMapLayerType.AdditionalTask)
            {
                if (_navigationMapHolder.Map.HasActiveLayerOfType(NavigationMapLayerType.AdditionalTask))
                {
                    _currentNavigationMapLayer = _navigationMapHolder.Map.GetLayer(currentRoundedPosition, NavigationMapLayerType.AdditionalTask);
                    return;
                }
            }
            
            _currentNavigationMapLayer = _navigationMapHolder.Map.GetLayer(currentRoundedPosition, NavigationMapLayerType.Main);

            return;
            
            bool PositionValidator(Vector2Int position)
            {
                return _navigationMapHolder.Map.NodeExists(position) && Vector2.Distance(currentRoundedPosition, position) <= 1f;
            }
        }
    }
}