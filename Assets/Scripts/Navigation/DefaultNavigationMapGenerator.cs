using Zenject;
using UnityEngine;
using WorldGeneration;
using System.Collections.Generic;

namespace Navigation
{
    public sealed class DefaultNavigationMapGenerator : MonoBehaviour
    {
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [Inject] private RoadMapHolder _roadMapHolder;
        [Inject] private IslandHeightMapHolder _heightMapHolder;
        private Vector2Int[] _checkDirections = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };
        private int _currentMapSize;
        private int _currentMapCenter;
        private bool[,] _touchedMap;

        private NavigationMap _nodeMap => _navigationMapHolder.Map;
        private bool[,] _roadMap => _roadMapHolder.Map;
        private int[,] _heightMap => _heightMapHolder.Map;

        public void GenerateDefaultNodeMap()
        {
            _currentMapSize = _roadMap.GetLength(0);
            _currentMapCenter = (_roadMap.GetLength(0) - 1) / 2;
            _touchedMap = new bool[_currentMapSize, _currentMapSize];
            
            GenerateStartingNode();

            StartMappingNodes();
        }

        private void GenerateStartingNode()
        {
            CreateNode(_currentMapCenter, _currentMapCenter);

            NavigationNode startingNode = new NavigationNode(new Vector3(_currentMapCenter, 0, _currentMapCenter));

            _nodeMap.GetNode(_currentMapCenter, _currentMapCenter).AddDefaultNode(startingNode);   
        }

        private void StartMappingNodes()
        {
            Vector2Int startingPosition = new Vector2Int(_currentMapCenter, _currentMapCenter);

            List<Vector2Int> startingNodesPositions = new();
            startingNodesPositions.Add(startingPosition);

            MapNodes(startingNodesPositions);
        }

        private void MapNodes(List<Vector2Int> nodePositions)
        {
            List<Vector2Int> foundNodePositions = new();
        
            for (int nodeIndex = 0; nodeIndex < nodePositions.Count; nodeIndex++)
            {
                int x = nodePositions[nodeIndex].x;
                int y = nodePositions[nodeIndex].y;
             
                NavigationNode currentNode = _nodeMap.GetNode(x, y);
             
                _touchedMap[x, y] = true;
                
                for (int i = 0; i < _checkDirections.Length; i++)
                {
                    int checkX = _checkDirections[i].x + x;
                    int checkY = _checkDirections[i].y + y;

                    if (PositionShouldBeMapped(checkX, checkY) == false) continue;

                    if (_nodeMap.GetNode(checkX, checkY) == null) CreateNode(checkX, checkY);

                    NavigationNode checkedNode = _nodeMap.GetNode(checkX, checkY);

                    if (checkedNode.ContainsDefaultNode(currentNode) == false)
                    {
                        checkedNode.AddDefaultNode(currentNode);

                        foundNodePositions.Add(new Vector2Int(checkX, checkY));
                    }
                }
            }

            if (foundNodePositions.Count > 0)
            {
                MapNodes(foundNodePositions);
            }
        }  

        private bool PositionShouldBeMapped(int x, int y)
        {
            if (IsValidPosition(x, y) == false) return false;
            
            return _roadMap[x, y] && _touchedMap[x, y] == false;
        }

        private bool IsValidPosition(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < _currentMapSize && y < _currentMapSize);
        }

        private void CreateNode(int x, int y)
        {
            int height = _heightMap[x, y];
            if (height < 1) height = 1;
            
            _nodeMap.SetNode(x, y, new NavigationNode(new Vector3(x, height + 1f, y)));
        }
    }
}