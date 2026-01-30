using TMPEffects.Extensions;
using UnityEngine;

namespace Navigation
{
    public sealed class MovementNavigationModule
    {
        private Transform _agentObject;
        private NavigationAgentData _agentData;
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private bool _nonsameHeightMovment;
        private AnimationCurve _currentHorizontalMovementCurve;
        private AnimationCurve _currrentVerticalMovementCurve;

        public MovementNavigationModule(Transform agentObject, NavigationAgentData navigationAgentData)
        {
            _agentObject = agentObject;
            _agentData = navigationAgentData;
            
            _startPosition = _agentObject.position;
        }    

        public void SetDestanation(Vector3 startPosition, Vector3 endPosition)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;

            _nonsameHeightMovment = Mathf.Abs(_startPosition.y - endPosition.y) >= 1f;

            if (_nonsameHeightMovment)
            {
                _currentHorizontalMovementCurve = _agentData.JumpHorizontalMovementCurve;

                if (startPosition.y > endPosition.y) _currrentVerticalMovementCurve = _agentData.JumpVerticalMovementCurve;
                else _currrentVerticalMovementCurve = new AnimationCurve(_agentData.JumpVerticalMovementCurve.keys).InvertCopy();
            }
            else _currentHorizontalMovementCurve = _agentData.DefaultHorizontalMovementCurve;
        }

        public void MoveTowardsNextPosition(float elapsedTime)
        {
            float evaluatedPosition = _currentHorizontalMovementCurve.Evaluate(elapsedTime);
            
            Vector3 resultPosition = Vector3.Lerp(_startPosition, _endPosition, evaluatedPosition);

            if (_nonsameHeightMovment)
            {
                resultPosition.y += _currrentVerticalMovementCurve.Evaluate(elapsedTime) * _agentData.VerticalCurveMultiplyer;
            }
            
            _agentObject.position = resultPosition;
        }   
    }
}