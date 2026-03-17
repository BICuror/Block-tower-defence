using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public abstract class RoadGenerationAlgorithm : ScriptableObject
    {
        protected IslandData _islandData;
        protected bool[,] _roadMap;
        private System.Random _random;
        private int _randomSeed;

        public abstract bool[,] GenerateRoadMap(Vector2Int[,] roadNodes, List<Vector2Int> spawnerNodes, IslandData islandData);

        public void SetRandomSeed(int seed)
        {
            _randomSeed = seed;
            _random = new System.Random(_randomSeed);
        }
        
        public int Random(int min, int max) => _random.Next(min, max);
        
        protected int NormalizeNumber(int num)
        {
            if (num == 0) return 0;
            return num / Mathf.Abs(num);
        }

        protected bool IsInBorders(Vector2Int position) 
        {
            return (position.x < _islandData.IslandSize && position.x >= 0 && position.y < _islandData.IslandSize && position.y >= 0);
        }
        
        protected Vector2Int FindSpawnerNodeIndex(Vector2Int position, Vector2Int[,] nodes, int amountOfNodes)
        {
            for (int x = 0; x < amountOfNodes; x++)
            {
                for(int y = 0; y < amountOfNodes; y++)
                {
                    if (position.x == nodes[x, y].x && position.y == nodes[x, y].y) return new Vector2Int(x, y);
                }
            }
            return new Vector2Int(0,0);
        }
        
        protected void MoveRoadTo(Vector2Int currentPos, Vector2Int neededPos)
        {
            Vector2Int directionsSummary = new Vector2Int();

            while(currentPos != neededPos)
            {
                Vector2Int moveDirection = GetMoveDirection(directionsSummary, currentPos, neededPos);

                directionsSummary += new Vector2Int(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.y));

                currentPos += moveDirection;

                _roadMap[currentPos.x, currentPos.y] = true;
            }
        }

        private Vector2Int GetMoveDirection(Vector2Int previousDirectionsSum, Vector2Int currentPos, Vector2Int neededPos)
        {
            Vector2Int dirDifference = neededPos - currentPos;

            int xDifference = NormalizeNumber(dirDifference.x);
            int yDifference = NormalizeNumber(dirDifference.y);

            if (xDifference == 0) return new Vector2Int(0, yDifference);
            if (yDifference == 0) return new Vector2Int(xDifference, 0);

            if (previousDirectionsSum.x > previousDirectionsSum.y) return new Vector2Int(0, yDifference);
            if (previousDirectionsSum.y > previousDirectionsSum.x) return new Vector2Int(xDifference, 0);
            
            if (Random(0, 100) >= 50f) return new Vector2Int(0, yDifference);
            return new Vector2Int(xDifference, 0);
        }
    }
}

