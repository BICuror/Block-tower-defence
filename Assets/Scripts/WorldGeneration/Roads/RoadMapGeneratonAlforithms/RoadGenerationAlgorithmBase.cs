using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public abstract class RoadGenerationAlgorithm : ScriptableObject
    {
        protected bool[,] _roadMap;
        protected IslandData _islandData;

        public abstract bool[,] GenerateRoadMap(Vector2Int[,] roadNodes, List<Vector2Int> spawnerNodes, IslandData islandData);

        protected int NormalizeNumber(int num)
        {
            if (num == 0) return 0;
            return num / Mathf.Abs(num);
        }

        protected bool IsInBorders(Vector2Int position) 
        {
            return (position.x < _islandData.IslandSize && position.x >= 0 && position.y < _islandData.IslandSize && position.y >= 0);
        }
    }
}

