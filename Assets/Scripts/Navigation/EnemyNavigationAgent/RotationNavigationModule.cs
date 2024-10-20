using UnityEngine;

namespace Navigation
{
    public sealed class RotationNavigationModule
    {
        private Transform _agentObject;
        private bool _shouldRotate;
        private Vector2Int _previousRotation;
        private Vector2Int _newRotation;
        private Vector2 _additionalRotationVector;

        public RotationNavigationModule(Transform agentObject, Vector2Int initialRotation)
        {
            _agentObject = agentObject;
            _previousRotation = initialRotation;
            _newRotation = initialRotation;

            _agentObject.LookAt(new Vector3(_agentObject.position.x + _previousRotation.x, _agentObject.position.y, _agentObject.position.z + _previousRotation.y));
        }

        public void SetPositions(Vector3 currentPosition, Vector3 nextPosition)
        {   
            _additionalRotationVector = Vector2Int.zero; 
            _previousRotation = _newRotation;

            Vector2 currentNodePosition = new Vector2(currentPosition.x, currentPosition.z);
            Vector2 nextNodePosition = new Vector2(nextPosition.x, nextPosition.z);

            _newRotation = new Vector2Int(Mathf.RoundToInt(nextNodePosition.x - currentNodePosition.x), Mathf.RoundToInt(nextNodePosition.y - currentNodePosition.y));
            
            _shouldRotate = _previousRotation != _newRotation;
            
            if (_shouldRotate)
            {
                if ((_previousRotation + _newRotation).magnitude == 0f) GetAdditionalRotationVector();
            }
        }

        public void RotateTowardsNode(float elapsedTime)
        {
            if (!_shouldRotate) return;

            Vector2 interpolatedPosition = Vector2.Lerp(_previousRotation, _newRotation, elapsedTime);

            interpolatedPosition = interpolatedPosition + (_additionalRotationVector * (-elapsedTime * elapsedTime + elapsedTime) * 2f);

            _agentObject.LookAt(new Vector3(_agentObject.position.x + interpolatedPosition.x, _agentObject.position.y, _agentObject.position.z + interpolatedPosition.y));
        }

        private void GetAdditionalRotationVector()
        {
            if (_newRotation.x != 0)
            {
                if (Random.Range(0, 100) > 50) _additionalRotationVector = new Vector2(0, 1f);
                else _additionalRotationVector = new Vector2(0, -1f);
            }
            else
            {
                if (Random.Range(0, 100) > 50) _additionalRotationVector = new Vector2(-1f, 0);
                else _additionalRotationVector = new Vector2(1f, 0);
            }
        }
    }
}