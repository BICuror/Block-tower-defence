using UnityEngine;
using Zenject;

namespace Navigation
{
    public sealed class NavigationNodeProvider : MonoBehaviour
    {
        private static NavigationNodeProvider _instance;
        [Inject] private NavigationMapHolder _navigationMap;

        public static NavigationNodeProvider Instance => _instance;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else Debug.LogError("Multiple instances of NavigationNodeProvider");
        }

        public NavigationNode GetNavigationNode(Vector3 position)
        {
            Vector2Int nodePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

            if (_navigationMap.Map.GetNode(nodePosition.x, nodePosition.y) == null) Debug.LogError($"{nodePosition.x}, {nodePosition.y}");
            return _navigationMap.Map.GetNode(nodePosition.x, nodePosition.y);
        }
    }
}