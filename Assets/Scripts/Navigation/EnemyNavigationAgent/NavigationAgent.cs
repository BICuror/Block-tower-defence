using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using System;

namespace Navigation
{
    public sealed class NavigationAgent : MonoBehaviour
    {
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [SerializeField] private Transform _rotationTarget;
        [Cached] private DraggableEntity _draggableEntity;
        [Cached] private Speed _speed;
        
        private CancellationTokenSource _movementCancellationTokenSource = new();
        private NavigationAgentData _agentData;
        private MovementNavigationModule _movementModule;
        private RotationNavigationModule _rotationModule;
        private NavigationAgentNodePicker _navigationAgentNodePicker;
        private NavigationMapLayer _currentNavigationMapLayer;
        private NavigationNode _startNode;
        private NavigationNode _endNode;
        private NavigationNode _nextNode;
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

        public void Disable() => _isEnabled = false;
        public void Enable() => _isEnabled = true;

        public void SetAgentData(NavigationAgentData agentData)
        {
            _agentData = agentData;
            _navigationAgentNodePicker = (NavigationAgentNodePicker)Activator.CreateInstance(Type.GetType(agentData.NavgationNodePickerType));
        } 
        
        public void Initialize()
        {
            _navigationMapHolder = NavigationMapHolder.Instance;
            
            StopMovement();
            FindSuitableLayer();
            AdaptToNavigationLayer();
            
            Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 nextPosition = new Vector2(_endNode.Position.x, _endNode.Position.z);

            Vector2Int previousRotation = new Vector2Int(-Mathf.RoundToInt(currentPosition.x - nextPosition.x), -Mathf.RoundToInt(currentPosition.y - nextPosition.y));
            
            _movementModule = new MovementNavigationModule(transform, _agentData);
            _rotationModule = new RotationNavigationModule(_rotationTarget, previousRotation);

            Enable();
            TravelToEndNode();
        }

        private void IterateToNextNode()
        {
            _startNode = _endNode;
            _endNode = _nextNode;   
            _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);
        }

        private void TravelToEndNode()
        {
            _movementModule.SetDestanation(_startNode.Position, _endNode.Position);
            _rotationModule.SetPositions(_endNode.Position, _nextNode.Position);

            TravelToNode();
        }

        private async void TravelToNode()
        {
            float elapsedTime = 0f;
            float distance = Vector2Int.Distance(_startNode.RoundedPosition, _endNode.RoundedPosition);
            
            if (!_currentNavigationMapLayer.IsEnabled) FindSuitableLayer();
            
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.fixedDeltaTime / _speed.Value / distance;

                _movementModule.MoveTowardsNextPosition(elapsedTime);
                _rotationModule.RotateTowardsNode(elapsedTime);

                try
                {
                    await UniTask.WaitForFixedUpdate(_movementCancellationTokenSource.Token);

                    if (!_isEnabled) await UniTask.WaitUntil(() => _isEnabled);
                }
                catch (Exception e)
                {
                    e.LogAsync();
                    return;
                }
            }

            if (!_currentNavigationMapLayer.IsEnabled) FindSuitableLayer();

            IterateToNextNode();
            TravelToEndNode();
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
                for (int i = 0; i < _checkDirection.Count; i++)
                {
                    if (_navigationMapHolder.Map.NodeExists(currentRoundedPosition + _checkDirection[i]))
                    {
                        currentRoundedPosition += _checkDirection[i];
                        break;
                    }
                }
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
        }

        private void AdaptToNavigationLayer()
        {
            Vector2Int currentRoundedPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

            _startNode = new NavigationNode(currentRoundedPosition, Mathf.RoundToInt(transform.position.y));
            _endNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, currentRoundedPosition);
            _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);
        }

        private void OnDisable() => StopMovement();
    }
}