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
            _navigationAgentNodePicker.SetWeightPickLogic(NavigationAgentNodePicker.WeightPickType.Minimal);
        }

        public void SetWeightPickLogic(NavigationAgentNodePicker.WeightPickType weightPickType)
        {
            _navigationAgentNodePicker.SetWeightPickLogic(weightPickType);
            StopMovement();
            _nextNode = _startNode;
            AdaptToNavigationLayer();
            TravelToEndNode();
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
        
        private void TravelToEndNode()
        {
            _movementModule.SetDestanation(new Vector3(transform.position.x, _startNode.Position.y, transform.position.z), _endNode.Position);
            _rotationModule.SetPositions(new Vector3(transform.position.x, _startNode.Position.y, transform.position.z), _nextNode.Position);

            TravelToNode();
        }

        private async void TravelToNode()
        {
            float elapsedTime = 0f;
            float duration = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), _endNode.RoundedPosition) * _speed.Value;
            
            if (!_currentNavigationMapLayer.IsEnabled) FindSuitableLayer();
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.fixedDeltaTime;

                float lerpValue = elapsedTime / duration;

                _movementModule.MoveTowardsNextPosition(lerpValue);
                _rotationModule.RotateTowardsNode(lerpValue);

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
            TravelToEndNode();
        }
        
        private void IterateToNextNode()
        {
            _startNode = _endNode;
            _endNode = _nextNode;   
            _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);
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