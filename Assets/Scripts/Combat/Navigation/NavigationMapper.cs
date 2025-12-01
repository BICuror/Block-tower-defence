using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;

namespace Navigation
{
    public sealed class NavigationMapper : MonoBehaviour
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
        
        private Dictionary<Vector2Int, int> _navigationWeightMap;

        private NavigationMap _nodeMap => _navigationMapHolder.Map;
        
        public Dictionary<Vector2Int, int> GenerateNavigationLayerWeights(Vector2Int initialPosition)
        {
            _navigationWeightMap = new();
            
            StartMappingNodes(initialPosition);
            
            return _navigationWeightMap;
        }
        
        private void StartMappingNodes(Vector2Int initialPosition)
        {
            List<Vector2Int> startingNodesPositions = new();
            startingNodesPositions.Add(initialPosition);

            MapNodeWeight(startingNodesPositions, 0);
        }

        private void MapNodeWeight(List<Vector2Int> positionsToWeight, int currentWeight)
        {
            List<Vector2Int> foundNodePositions = new();
        
            for (int nodeIndex = 0; nodeIndex < positionsToWeight.Count; nodeIndex++)
            {
                Vector2Int nodePos = new Vector2Int(positionsToWeight[nodeIndex].x, positionsToWeight[nodeIndex].y);
                
                _navigationWeightMap.Add(nodePos, currentWeight);
                
                for (int i = 0; i < _checkDirections.Length; i++)
                {
                    Vector2Int checkPos = new Vector2Int((_checkDirections[i] + nodePos).x, (_checkDirections[i] + nodePos).y);

                    if (_nodeMap.NodeExists(checkPos) && !_navigationWeightMap.ContainsKey(checkPos) && !foundNodePositions.Contains(checkPos))
                    {
                        foundNodePositions.Add(checkPos);
                    }
                }
            }

            if (foundNodePositions.Count > 0)
            {
                currentWeight++;
                MapNodeWeight(foundNodePositions, currentWeight);
            }
        }
    }
}