using UnityEngine;
using System.Collections;
using Cashing;
using Combat;

namespace Navigation
{
    public sealed class NavigationAgent : MonoBehaviour
    {
        [Cached] private DraggableEntity _draggableEntity;
        [Cached] private Speed _speed;
        private NavigationAgentData _agentData;
        private MovmentNavigationModule _movmentModule;
        private RotationNavigationModule _rotationModule;
        
        private NavigationNode _startNode;
        private NavigationNode _endNode;
        private NavigationNode _nextNode;

        private void Start()
        {
            _draggableEntity.PickedUp += StopAllCoroutines;
            _draggableEntity.Placed += Initialize;
        }
        public void SetAgentData(NavigationAgentData agentData) => _agentData = agentData;
        public void Initialize()
        {
            StopAllCoroutines();

            _startNode = NavigationNodeProvider.Instance.GetNavigationNode(transform.position);
            _endNode = _startNode.GetNextNode();
            _nextNode = _endNode.GetNextNode();

            Vector2 currentPosition = new Vector2(_startNode.Position.x, _startNode.Position.z);
            Vector2 nextPosition = new Vector2(_endNode.Position.x, _endNode.Position.z);

            Vector2Int previousRotation = new Vector2Int(Mathf.RoundToInt(currentPosition.x - nextPosition.x), Mathf.RoundToInt(currentPosition.y - nextPosition.y));
            
            _movmentModule = new MovmentNavigationModule(transform, _agentData);
            _rotationModule = new RotationNavigationModule(transform, previousRotation);

            TravelToEndNode();
        }

        private void IterateToNextNode()
        {
            _startNode = _endNode;
            _endNode = _nextNode;   
            _nextNode = _nextNode.GetNextNode();
        }

        private void TravelToEndNode()
        {
            _movmentModule.SetDestanation(_startNode.Position, _endNode.Position);
            _rotationModule.SetPositions(_endNode.Position, _nextNode.Position);

            StartCoroutine(TravelToNode());
        }

        private IEnumerator TravelToNode()
        {
            float elapsedTime = 0f;
            float evaluatedTime = 0f;
            while (elapsedTime <= _speed.Value)
            {
                elapsedTime += Time.deltaTime;
                evaluatedTime = elapsedTime / _speed.Value;

                _movmentModule.MoveTowardsNextPosition(evaluatedTime);
                _rotationModule.RotateTowardsNode(evaluatedTime);

                yield return new WaitForFixedUpdate();
            } 

            IterateToNextNode();
            TravelToEndNode();
        }
    }
}