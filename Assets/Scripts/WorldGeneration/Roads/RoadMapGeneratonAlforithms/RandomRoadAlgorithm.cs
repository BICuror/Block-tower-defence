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

        public override bool[,] GenerateRoadMap(Vector2Int[,] roadNodes, List<Vector2Int> spawnerNodes, IslandData islandData)
        {
            _islandData = islandData;

            _roadMap = new bool[_islandData.IslandSize, _islandData.IslandSize];

            for (int i = 0; i < spawnerNodes.Count; i++)
            {  
                Vector2Int currentSpawnerPosition = FindSpawnerNodeIndex(spawnerNodes[i], roadNodes, _islandData.AmountOfRoadNodes);

                CreateRandomSpawnerRoad(currentSpawnerPosition, roadNodes);
            }

            return _roadMap;
        }

        private Vector2Int FindSpawnerNodeIndex(Vector2Int position, Vector2Int[,] nodes, int amountOfNodes)
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


        private void CreateRandomSpawnerRoad(Vector2Int currentPosition, Vector2Int[,] roadNodes)
        {
            currentPosition = roadNodes[currentPosition.x, currentPosition.y];

            int middleIndex = (_islandData.IslandSize - 1) / 2;

            Vector2Int centerPosition = new Vector2Int(middleIndex, middleIndex);

            int iteration = 0;

            int createdTiles = 0;

            while(centerPosition != currentPosition)
            {
                iteration++;

                _roadMap[currentPosition.x, currentPosition.y] = true;

                Vector2Int direction = new Vector2Int();

                bool closeToMainBuilding = Vector2Int.Distance(currentPosition, centerPosition) <= _snapToMainBuildingDistance;

                bool shouldUseRandomDirection = (closeToMainBuilding == false) && ShouldUseRandomDirection(iteration) && createdTiles <= _maxRandomTiles;

                if (shouldUseRandomDirection)
                {
                    Vector2Int randomDirection = _randomDirections[Random.Range(0, _randomDirections.Length)];

                    if (IsInBorders(randomDirection + currentPosition)) direction = randomDirection;
                }
                else
                {
                    direction = new Vector2Int(NormalizeNumber(middleIndex - currentPosition.x), NormalizeNumber(middleIndex - currentPosition.y));
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

                _roadMap[currentPosition.x, currentPosition.y] = true;
            }
        }

        private bool ShouldUseRandomDirection(int currentIteration)
        {
            if (currentIteration < _maxIteraions && Random.Range(0f, 1f) <= _chanseForRandomDirection) return true;
            else if (currentIteration >= _maxIteraions && Random.Range(0f, 1f) <= _chanseForRandomDirectionAfterAllIterations) return true;
            else return false;
        }

        private bool HasFutureMoves(Vector2Int position, int iteration)
        {
            for (int moveIndex = 0; moveIndex < _checkDirections.Length; moveIndex++)
            {
                Vector2Int currentPosition = _checkDirections[moveIndex] + position;

                if (IsInBorders(currentPosition) && CheckForNerbyRoadPositions(currentPosition)) 
                {
                    if (iteration == 0) return true;

                    bool hadRoadTile = _roadMap[currentPosition.x, currentPosition.y];

                    _roadMap[currentPosition.x, currentPosition.y] = true;
                    
                    bool hasMoves = HasFutureMoves(currentPosition, iteration - 1);
                    
                    _roadMap[currentPosition.x, currentPosition.y] = hadRoadTile;
                
                    if (hasMoves) return true;
                }
                else 
                {
                    continue;
                }
            }

            return false;
        }

        private bool CheckForNerbyRoadPositions(Vector2Int position)
        {
            int count = 0;

            for (int i = 0; i < _checkDirections.Length; i++)
            {
                Vector2Int currentCheckPosition = position + _checkDirections[i];
                if (IsInBorders(currentCheckPosition) && _roadMap[currentCheckPosition.x, currentCheckPosition.y]) count++;
            }

            return count == _allowedBlocksNerby;
        }
    }
}