using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "RandomRoadAlgorithm", menuName = "Generation/RoadMapGeneratoionAlgorithm/RandomRoadAlgorithm")]

    public sealed class RandomRoadAlgorithm : RoadGenerationAlgorithm
    {
        [SerializeField] private int _searchIterations = 4;
        [SerializeField] private int _snapToMainBuildingDistance = 3;
        [SerializeField] private int _maxIteraions = 10000;
        [SerializeField] private int _allowedBlocksNerby = 1;
        
        [Range(0f, 1f)] [SerializeField] private float _chanseForRandomDirection = 0.75f;
        [Range(0f, 1f)] [SerializeField] private float _chanseForRandomDirectionAfterAllIterations = 0.25f;
        [SerializeField] private int _maxRandomTiles = 25;
        
        private Vector2Int[] _randomDirections = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private Vector2Int[] _checkDirections = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        public override bool TryGenerateRoadMap(out bool[,] roadMap)
        {
            CreateRandomSpawnerRoad(currentSpawnerPosition, roadNodes);
        }

        private void CreateRandomSpawnerRoad(Vector2Int currentPosition, Vector2Int[,] roadNodes)
        {
            currentPosition = roadNodes[currentPosition.x, currentPosition.y];

            int iteration = 0;

            int createdTiles = 0;

            while(EndPosition != currentPosition)
            {
                iteration++;

                RoadMap[currentPosition.x, currentPosition.y] = true;

                Vector2Int direction = new Vector2Int();

                bool closeToMainBuilding = Vector2Int.Distance(currentPosition, EndPosition) <= _snapToMainBuildingDistance;

                bool shouldUseRandomDirection = (closeToMainBuilding == false) && ShouldUseRandomDirection(iteration) && createdTiles <= _maxRandomTiles;

                if (shouldUseRandomDirection)
                {
                    Vector2Int randomDirection = _randomDirections[Random.Range(0, _randomDirections.Length)];

                    if (IsInBorders(randomDirection + currentPosition)) direction = randomDirection;
                }
                else
                {
                    direction = new Vector2Int(NormalizeNumber(EndPosition.x - currentPosition.x), NormalizeNumber(EndPosition.y - currentPosition.y));
                }
                
                if (direction.x != 0 && direction.y != 0)
                {
                    if (Random.Range(0, 100) > 50) direction.x = 0;
                    else direction.y = 0;
                }

                if (iteration < _maxIteraions && closeToMainBuilding == false)
                {
                    if (HasFutureMoves(currentPosition + direction, _searchIterations) == false) continue;
                }
                
                currentPosition += direction;

                createdTiles++;

                RoadMap[currentPosition.x, currentPosition.y] = true;
            }
        }

        private bool ShouldUseRandomDirection(int currentIteration)
        {
            if (currentIteration < _maxIteraions && Random.Range(0f, 1f) <= _chanseForRandomDirection) return true;
            if (currentIteration >= _maxIteraions && Random.Range(0f, 1f) <= _chanseForRandomDirectionAfterAllIterations) return true;
            return false;
        }

        private bool HasFutureMoves(Vector2Int position, int iteration)
        {
            for (int moveIndex = 0; moveIndex < _checkDirections.Length; moveIndex++)
            {
                Vector2Int currentPosition = _checkDirections[moveIndex] + position;

                if (IsInBorders(currentPosition) && CheckForNerbyRoadPositions(currentPosition)) 
                {
                    if (iteration == 0) return true;

                    bool hadRoadTile = RoadMap[currentPosition.x, currentPosition.y];

                    RoadMap[currentPosition.x, currentPosition.y] = true;
                    
                    bool hasMoves = HasFutureMoves(currentPosition, iteration - 1);
                    
                    RoadMap[currentPosition.x, currentPosition.y] = hadRoadTile;
                
                    if (hasMoves) return true;
                }
            }

            return false;
        }
    }
}