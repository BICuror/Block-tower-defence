using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public abstract class RoadGenerationAlgorithm : ScriptableObject
    {
        protected bool[,] RoadMap;
        protected IslandData IslandData;
        protected Vector2Int[,] RoadNodes;
        protected Vector2Int StartPosition;
        protected Vector2Int EndPosition;
        protected bool[,] TempRoadmap;
        
        protected readonly Vector2Int[] CheckDirections = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left, 
            Vector2Int.right
        };

        public void Initialize(IslandData islandData, Vector2Int[,] roadNodes, bool[,] roadMap)
        {
            IslandData = islandData;
            RoadMap = roadMap;
            RoadNodes = roadNodes;
            
            TempRoadmap = new bool[IslandData.IslandSize, IslandData.IslandSize];
        }

        public void SetPositions(Vector2Int startPosition, Vector2Int endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
        
        public abstract bool TryGenerateRoadMap();

        public bool[,] GetRoadMap() => TempRoadmap;
        
        protected int NormalizeNumber(int num)
        {
            if (num == 0) return 0;
            if (num < 0) return -1;
            return 1;
        }

        protected bool IsInBorders(Vector2Int position) 
        {
            return (position.x < IslandData.IslandSize && position.x >= 0 && position.y < IslandData.IslandSize && position.y >= 0);
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
        
        protected void ConnectPoints(Vector2Int currentPos, Vector2Int neededPos)
        {
            Vector2Int directionsSummary = new Vector2Int();

            while(currentPos != neededPos)
            {
                Vector2Int moveDirection = GetMoveDirection(directionsSummary, currentPos, neededPos);

                directionsSummary += new Vector2Int(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.y));

                currentPos += moveDirection;

                TempRoadmap[currentPos.x, currentPos.y] = true;
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
            
            if (Random.value >= 0.5f) return new Vector2Int(0, yDifference);
            return new Vector2Int(xDifference, 0);
        }
        
        private bool HasTilesNearby(Vector2Int position, bool checkRoadMap = false)
        {
            return GetNearbyRoadCount(position, checkRoadMap) == 0;
        }
        
        private int GetNearbyRoadCount(Vector2Int position, bool checkRoadMap = false)
        {
            int count = 0;

            for (int i = 0; i < CheckDirections.Length; i++)
            {
                Vector2Int checkPosition = CheckDirections[i] + position;

                if (IsInBorders(checkPosition))
                {
                    if (CheckRoadPosition(checkPosition) || CheckTempMap(checkPosition)) count++;
                }
            }
            
            return count;
            
            bool CheckRoadPosition(Vector2Int position) => checkRoadMap && RoadMap[position.x, position.y];
            bool CheckTempMap(Vector2Int position) => TempRoadmap[position.x, position.y];
        }
    }
}