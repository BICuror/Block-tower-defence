using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Navigation
{
    public sealed class NavigationAgent : DraggableObject
    {
        private NavigationAgentData _agentData;
        private MovmentNavigationModule _movmentModule;
        private RotationNavigationModule _rotationModule;
        private NavigationNode _currentNode;
        private NavigationNode _nextNode;
        private NavigationNode _incomingNode;

        private void Start()
        {
            PickedUp.AddListener(StopAllCoroutines);
            Placed.AddListener(IterateToNextNode);
        }

        public void Init(NavigationAgentData agentData)
        {
            StopAllCoroutines();
            _agentData = agentData;

            _currentNode = NavigationNodeProvider.Instance.GetNavigationNode(transform.position);
            _nextNode = _currentNode.GetNextNode();
            _incomingNode = _nextNode.GetNextNode();

            Vector2 currentPosition = new Vector2(_currentNode.Position.x, _currentNode.Position.z);
            Vector2 nextPosition = new Vector2(_nextNode.Position.x, _nextNode.Position.z);

            Vector2Int previousRotation = new Vector2Int(Mathf.RoundToInt(currentPosition.x - nextPosition.x), Mathf.RoundToInt(currentPosition.y - nextPosition.y));
            
            _movmentModule = new MovmentNavigationModule(transform, _agentData);
            _rotationModule = new RotationNavigationModule(transform, previousRotation);

            _movmentModule.SetDestanation(_currentNode.Position, _nextNode.Position);
            _rotationModule.SetPositions(_nextNode.Position, _incomingNode.Position);

            StartCoroutine(TravelToNode());
        }

        private void IterateToNextNode()
        {
            _currentNode = _nextNode;
            _nextNode = _incomingNode;   
            _incomingNode = _incomingNode.GetNextNode();

            _movmentModule.SetDestanation(_currentNode.Position, _nextNode.Position);
            _rotationModule.SetPositions(_nextNode.Position, _incomingNode.Position);

            StartCoroutine(TravelToNode());
        }

        private IEnumerator TravelToNode()
        {
            float elapsedTime = 0f;
            float evaluatedTime = 0f;
            while (elapsedTime <= _agentData.TimePerBlock)
            {  
                elapsedTime += Time.deltaTime;
                evaluatedTime = elapsedTime / _agentData.TimePerBlock;

                _movmentModule.MoveTowardsNextPosition(evaluatedTime);
                _rotationModule.RotateTowardsNode(evaluatedTime);

                yield return new WaitForFixedUpdate();
            } 

            IterateToNextNode();
        }
    }
}