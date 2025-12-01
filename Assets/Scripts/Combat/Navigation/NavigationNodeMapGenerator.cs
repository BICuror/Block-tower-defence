using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;

namespace Navigation
{
    public sealed class NavigationNodeMapGenerator : MonoBehaviour
    {
        private readonly Vector2Int[] _checkDirections = new Vector2Int[4]
        { 
            Vector2Int.up, 
            Vector2Int.down, 
            Vector2Int.right, 
            Vector2Int.left
        };
        
        [Inject] private NavigationMapHolder _navigationMapHolder;
        [Inject] private IslandHeightMapHolder _heightMapHolder;
        [Inject] private RoadMapHolder _roadMapHolder;
        
        private int _currentMapSize;
        private int _currentMapCenter;
        private bool[,] _roadMap => _roadMapHolder.Map;
        private int[,] _heightMap => _heightMapHolder.Map;

        private NavigationMap _nodeMap => _navigationMapHolder.Map;

        public void GenerateNodeMap()
        {
            _currentMapSize = _roadMap.GetLength(0);
            _currentMapCenter = (_roadMap.GetLength(0) - 1) / 2;

            StartMappingNodes();
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
                Vector2Int nodePos = new Vector2Int(nodePositions[nodeIndex].x, nodePositions[nodeIndex].y);
                
                for (int i = 0; i < _checkDirections.Length; i++)
                {
                    Vector2Int checkPos = new Vector2Int((_checkDirections[i] + nodePos).x, (_checkDirections[i] + nodePos).y);

                    if (!NodeShouldBeCreatedOnPosition(checkPos)) continue;

                    CreateNode(checkPos);
                    
                    foundNodePositions.Add(checkPos);
                }
            }

            if (foundNodePositions.Count > 0)
            {
                MapNodes(foundNodePositions);
            }
        }  

        private bool NodeShouldBeCreatedOnPosition(Vector2Int checkPos)
        {
            if (!IsValidPosition(checkPos)) return false;
            if (!_roadMap[checkPos.x, checkPos.y]) return false;
            return !_nodeMap.NodeExists(checkPos);
        }

        private bool IsValidPosition(Vector2Int checkPos)
        {
            return (checkPos.x >= 0 && checkPos.y >= 0 && checkPos.x < _currentMapSize && checkPos.y < _currentMapSize);
        }

        private void CreateNode(Vector2Int nodePosition)
        {
            int height = _heightMap[nodePosition.x, nodePosition.y];
            if (height < 1) height = 1;
            height++;

            NavigationNode node = new(nodePosition, height);
            
            _nodeMap.SetNode(nodePosition, node);
        }
    }
}