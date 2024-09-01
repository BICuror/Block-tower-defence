using Zenject;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration;

namespace Navigation
{
    public sealed class OptionalNavigationMapGenerator : MonoBehaviour
    {
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [Inject] private RoadMapHolder _roadMapHolder;
        [Inject] private IslandHeightMapHolder _heightMapHolder;
        private int _currentMapSize;
        private int _currentMapCenter;
        private bool[,] _touchedMap;
        private Vector2Int[] _checkDirections = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };

        private NavigationMap _nodeMap => _navigationMapHolder.Map;
        private bool[,] _roadMap => _roadMapHolder.Map;
        private int[,] _heightMap => _heightMapHolder.Map;

        public void GenerateOptionalNodeMap(List<Vector2Int> startingPositions, List<INavigationCondition> conditions, List<Vector2Int> endingPositions)
        {
            _currentMapSize = _roadMap.GetLength(0);
            _currentMapCenter = (_roadMap.GetLength(0) - 1) / 2;

            for (int i = 0; i < conditions.Count; i++)
            {   
                _touchedMap = new bool[_currentMapSize, _currentMapSize];
                if (NewOptionalIterateThoruNode(startingPositions[i], conditions[i], endingPositions[i]) == false) 
                {
                    Debug.LogError("Couldn't find a path from position to spawner");
                }
            }
        }

        private bool NewOptionalIterateThoruNode(Vector2Int currentPosition, INavigationCondition condition, Vector2Int endPosition)
        {
            if (currentPosition == endPosition) return true;

            int x = currentPosition.x;
            int y = currentPosition.y;
            Debug.Log($"{x}, {y}");

            if (ValidRoadNode(x, y) == false) return false;

            Debug.Log($"{x}, {y}");
            
            _touchedMap[x, y] = true;

            MapNearbyNodes(x, y, condition);
            
            int xDir = 1;
            if (x < _currentMapCenter) xDir = -1;
            int yDir = 1;
            if (y < _currentMapCenter) yDir = -1;

            if (ShouldMapNode(x + xDir, y) && NewOptionalIterateThoruNode(currentPosition + new Vector2Int(xDir, 0), condition, endPosition)) return true;
            if (ShouldMapNode(x, y + yDir) && NewOptionalIterateThoruNode(currentPosition + new Vector2Int(0, yDir), condition, endPosition)) return true;
            if (ShouldMapNode(x - xDir, y) && NewOptionalIterateThoruNode(currentPosition + new Vector2Int(-xDir, 0), condition, endPosition)) return true;
            if (ShouldMapNode(x, y - yDir) && NewOptionalIterateThoruNode(currentPosition + new Vector2Int(0, -yDir), condition, endPosition)) return true;

            return false;
        }

        private void MapNearbyNodes(int x, int y, INavigationCondition condition) 
        {
            NavigationNode currentNode = _nodeMap.GetNode(x, y);
            
            foreach (Vector2Int checkDirection in _checkDirections)
            {
                int checkX = checkDirection.x + x;
                int checkY = checkDirection.y + y;

                if (ShouldMapNode(checkX, checkY) == false) continue;

                if (_nodeMap.GetNode(checkX, checkY) == null) CreateNode(checkX, checkY);

                NavigationNode checkedNode = _nodeMap.GetNode(checkX, checkY);

                if (checkedNode.ContainsOptionalNode(condition, currentNode) == false)
                {
                    checkedNode.AddOptionalNode(condition, currentNode);
                } 
            }
        }

        private bool PositionShouldBeChecked(int x, int y)
        {
            return IsValidPosition(x, y) && _roadMap[x, y];
        }

        private bool ShouldMapNode(int x, int y)
        {
            if (IsValidPosition(x, y) == false) return false;

            return ValidRoadNode(x, y);
        }

        private bool ValidRoadNode(int x, int y)
        {
            if (_touchedMap[x, y]) return false;

            return _roadMap[x, y];
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _currentMapSize && y < _currentMapSize;
        }

        private void CreateNode(int x, int y)
        {
            int height = _heightMap[x, y];
            if (height < 1) height = 1;
            
            _nodeMap.SetNode(x, y, new NavigationNode(new Vector3(x, height + 1f, y)));
        }
    }
}
