using UnityEngine;

namespace Navigation
{
    public sealed class NavigationNode
    {
        public NavigationNode(Vector2Int roundedPosition, int height)
        {   
            _roundedPosition = roundedPosition;
            _position = new Vector3(roundedPosition.x, height, roundedPosition.y);
        }

        private Vector2Int _roundedPosition;
        private Vector3 _position;
        
        public Vector2Int RoundedPosition => _roundedPosition;
        public Vector3 Position => _position;
    }
}