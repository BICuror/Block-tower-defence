using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cashing;
using Zenject;
using Combat;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Navigation
{
    public sealed class NavigationAgent : MonoBehaviour
    {
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [Cached] private DraggableEntity _draggableEntity;
        [Cached] private Speed _speed;
        
        private CancellationTokenSource _movementCancellationTokenSource = new();
        
        private NavigationAgentData _agentData;
        
        private MovmentNavigationModule _movmentModule;
        private RotationNavigationModule _rotationModule;
        private NavigationAgentNodePicker _navigationAgentNodePicker;
        
        private NavigationMapLayer _currentNavigationMapLayer;
        private NavigationNode _startNode;
        private NavigationNode _endNode;
        private NavigationNode _nextNode;

        private void Start()
        {
            _draggableEntity.PickedUp += StopMovement;
            _draggableEntity.Placed += Initialize;
        }

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
            
            Vector2 currentPosition = new Vector2(_startNode.Position.x, _startNode.Position.z);
            Vector2 nextPosition = new Vector2(_endNode.Position.x, _endNode.Position.z);

            Vector2Int previousRotation = new Vector2Int(-Mathf.RoundToInt(currentPosition.x - nextPosition.x), -Mathf.RoundToInt(currentPosition.y - nextPosition.y));
            
            _movmentModule = new MovmentNavigationModule(transform, _agentData);
            _rotationModule = new RotationNavigationModule(transform, previousRotation);

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
            _movmentModule.SetDestanation(_startNode.Position, _endNode.Position);
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

                _movmentModule.MoveTowardsNextPosition(elapsedTime);
                _rotationModule.RotateTowardsNode(elapsedTime);

                try
                {
                    await UniTask.WaitForFixedUpdate(_movementCancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    TaskUtility.LogAsync(e);
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

            _startNode = _navigationMapHolder.Map.GetNode(currentRoundedPosition);
            _endNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, currentRoundedPosition);
            _nextNode = _navigationAgentNodePicker.PickNavigationNode(_navigationMapHolder.Map, _currentNavigationMapLayer, _endNode.RoundedPosition);
        }

        private void OnDisable() => StopMovement();

        private List<Vector2Int> _checkDirections = new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };
    }
}