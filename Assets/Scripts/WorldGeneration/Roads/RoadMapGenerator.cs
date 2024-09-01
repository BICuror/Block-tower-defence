using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class RoadMapGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
        [Inject] private RoadNodeGenerator _roadNodeGenerator;
        [Inject] private RoadMapHolder _roadMapHolder;
        [SerializeField] private int _initialRaduis = 4;
        [SerializeField] private float _minimalDistanceToSpawner = 10f;
        public bool[,] optionalMap; 
        private bool[,] searchMap;

        public Vector2Int chestPosition;
        public List<Vector2Int> chestPositions = new();
        public List<Vector2Int> GetEnemyPositions() => _enemyBiomeContainer.GetEnemyBiomesPositions();

        private Vector2Int[] _checkDirections = new Vector2Int[4]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.left
        };


        public void GenerateRoads()
        {
            List<Vector2Int> spawnerNodes = _enemyBiomeContainer.GetEnemyBiomesPositions();

            Vector2Int[,] roadNodes = _roadNodeGenerator.GetAllNodes();

            bool[,] roadMap = _islandData.RoadMapGenerationAlgorithm.GenerateRoadMap(roadNodes, spawnerNodes, _islandData);
            
            _roadMapHolder.SetRoadMap(roadMap);
            //GetOptionalRoadMap();

            AddCenterRoad();            
        }

        private void AddCenterRoad()
        {
            bool[,] roadMap = _roadMapHolder.Map;

            int centerIndex = Mathf.RoundToInt((_islandData.IslandSize - 1) / 2);

            for (int x = centerIndex - 1; x <= centerIndex + 1; x++)
            {
                for (int y = centerIndex - 1; y <= centerIndex + 1; y++)
                {
                    roadMap[x, y] = true;
                }
            }

            _roadMapHolder.SetRoadMap(roadMap);
        }

        private bool[,] GetOptionalRoadMap()
        {
            int centerIndex = Mathf.RoundToInt((_islandData.IslandSize - 1) / 2);

            bool[,] roadMap = _roadMapHolder.Map;
            optionalMap = new bool[roadMap.GetLength(0), roadMap.GetLength(0)];
            
            List<Vector2Int> spawnerNodes = _enemyBiomeContainer.GetEnemyBiomesPositions();

            chestPositions = new();

            for (int chestIndex = 0; chestIndex < spawnerNodes.Count; chestIndex++)
            {
                int interations = 0;
                while (interations < 100)
                {
                    interations++;
                    chestPosition = FindChestPosition(new Vector2Int(centerIndex, centerIndex), spawnerNodes[chestIndex]);
                    
                    Vector2Int roadPosition = new Vector2Int(0, 0);
                    bool neededRoadPositionFound = false;
                    List<Vector2Int> positionsToIterateThrough = new();
                    List<Vector2Int> nextPositionsToIterateThrough = new();
                    positionsToIterateThrough.Add(chestPosition);

                    int iteration = 0;

                    bool[,] touchedMap = new bool[roadMap.GetLength(0), roadMap.GetLength(0)];
                    while (neededRoadPositionFound == false && iteration < 100)
                    {
                        iteration++;
                        for (int i = 0; i < positionsToIterateThrough.Count; i++)
                        {
                            if (neededRoadPositionFound) break;
                            touchedMap[positionsToIterateThrough[i].x, positionsToIterateThrough[i].y] = true;

                            for (int dir = 0; dir < _checkDirections.Length; dir++)
                            {
                                if (neededRoadPositionFound) break;
                                int x = positionsToIterateThrough[i].x + _checkDirections[dir].x;
                                int y = positionsToIterateThrough[i].y + _checkDirections[dir].y;

                                if (IsInBounds(x, y) && (roadMap[x, y] == false) && (touchedMap[x, y] == false))
                                {
                                    nextPositionsToIterateThrough.Add(new Vector2Int(x, y));
                                }
                                else if (IsInBounds(x, y) && (roadMap[x, y])) 
                                {
                                    roadPosition = new Vector2Int(x, y);
                                    neededRoadPositionFound = true;
                                }
                            }
                        }

                        positionsToIterateThrough = nextPositionsToIterateThrough;
                        nextPositionsToIterateThrough = new();
                    }
                
                    Vector2Int currentPosition = chestPosition;
                    Vector2Int neededPosition = roadPosition;

                    iteration = 0;

                    List<Vector2Int> neededPositions = new List<Vector2Int>();

                    while (currentPosition != neededPosition && iteration < 100)
                    {
                        iteration++;
                        Vector2Int difference = neededPosition - currentPosition;

                        int xDif = difference.x;
                        int yDif = difference.y;

                        if (xDif > 0 && xDif >= 1) xDif = 1;
                        else if (xDif < 0 && xDif <= -1) xDif = -1;

                        if (yDif > 0 && yDif >= 1) yDif = 1;
                        else if (yDif < 0 && yDif <= -1) yDif = -1;

                        if (xDif != 0 && yDif != 0)
                        {
                            if (Random.Range(0, 100) > 50) xDif = 0;
                            else yDif = 0;
                        }

                        neededPositions.Add(currentPosition);
                        optionalMap[currentPosition.x, currentPosition.y] = true;

                        currentPosition = currentPosition + new Vector2Int(xDif, yDif);

                        optionalMap[currentPosition.x, currentPosition.y] = true;
                        neededPositions.Add(currentPosition);
                    }

                    searchMap = new bool[_islandData.IslandSize,_islandData.IslandSize];

                    if (HasRoadToSpawner(chestPosition, spawnerNodes[chestIndex]))
                    {
                        for (int i = 0; i < neededPositions.Count; i++)
                        {
                            roadMap[neededPositions[i].x, neededPositions[i].y] = true;
                        }

                        MakeAnOptionalMap(chestPosition, spawnerNodes[chestIndex]);
                    
                        _roadMapHolder.SetRoadMap(roadMap);

                        chestPositions.Add(chestPosition);

                        interations = 1000;
                    }
                    else
                    {
                        for (int i = 0; i < neededPositions.Count; i++)
                        {           
                            optionalMap[neededPositions[i].x, neededPositions[i].y] = false;
                        }
                    }          
                }
            }

            return optionalMap;
        }

        private bool HasRoadToSpawner(Vector2Int currentPosition, Vector2Int endPosition)
        {
            if (currentPosition == endPosition) return true;
            if (searchMap[currentPosition.x, currentPosition.y] == true) return false;
            else searchMap[currentPosition.x, currentPosition.y] = true;
            int centerIndex = Mathf.RoundToInt((_islandData.IslandSize - 1) / 2);
            if (Vector2Int.Distance(currentPosition, new Vector2Int(centerIndex, centerIndex)) < 2f) return false;
            
            int xDir = 1;
            if (currentPosition.x < centerIndex) xDir = -1;

            int yDir = 1;
            if (currentPosition.y < centerIndex) yDir = -1;

            if (IsInBoundsAndRoadOptional(currentPosition.x + xDir, currentPosition.y) && HasRoadToSpawner(currentPosition + new Vector2Int(xDir, 0), endPosition)) return true;
            if (IsInBoundsAndRoadOptional(currentPosition.x, currentPosition.y + yDir) && HasRoadToSpawner(currentPosition + new Vector2Int(0, yDir), endPosition)) return true;
            if (IsInBoundsAndRoadOptional(currentPosition.x - xDir, currentPosition.y) && HasRoadToSpawner(currentPosition + new Vector2Int(-xDir, 0), endPosition)) return true;
            if (IsInBoundsAndRoadOptional(currentPosition.x, currentPosition.y - yDir) && HasRoadToSpawner(currentPosition + new Vector2Int(0, -yDir), endPosition)) return true;
            return false;
        }

        private bool MakeAnOptionalMap(Vector2Int currentPosition, Vector2Int endPosition)
        {
            if (currentPosition == endPosition) return true;
            if (optionalMap[currentPosition.x, currentPosition.y] || _roadMapHolder.Map[currentPosition.x, currentPosition.y] == false) return false;
            else optionalMap[currentPosition.x, currentPosition.y] = true;

            int centerIndex = Mathf.RoundToInt((_islandData.IslandSize - 1) / 2);
            if (Vector2Int.Distance(currentPosition, new Vector2Int(centerIndex, centerIndex)) < 3f) return false;
            
            int xDir = 1;
            if (currentPosition.x < centerIndex) xDir = -1;

            int yDir = 1;
            if (currentPosition.y < centerIndex) yDir = -1;

            if (IsInBoundsAndRoad(currentPosition.x + xDir, currentPosition.y) && MakeAnOptionalMap(currentPosition + new Vector2Int(xDir, 0), endPosition)) return true;
            if (IsInBoundsAndRoad(currentPosition.x, currentPosition.y + yDir) && MakeAnOptionalMap(currentPosition + new Vector2Int(0, yDir), endPosition)) return true;
            if (IsInBoundsAndRoad(currentPosition.x - xDir, currentPosition.y) && MakeAnOptionalMap(currentPosition + new Vector2Int(-xDir, 0), endPosition)) return true;
            if (IsInBoundsAndRoad(currentPosition.x, currentPosition.y - yDir) && MakeAnOptionalMap(currentPosition + new Vector2Int(0, -yDir), endPosition)) return true;

            return false;
        }

        private Vector2Int FindChestPosition(Vector2Int centerPosition, Vector2Int spawnerPosition)
        {
            for (int radius = _initialRaduis; radius > 1; radius--)
            {
                List<Vector2Int> possibleChestPositions = new();

                for (int x = -radius; x < radius; x++)
                {
                    for (int y = -radius; y < radius; y++)
                    {                        
                        if ((x == -radius || x == radius - 1) || (y == -radius || y == radius - 1))
                        {
                            int checkX = x + centerPosition.x;
                            int checkY = y + centerPosition.y;

                            if (IsInBounds(checkX, checkY))
                            {
                                bool hasRoadAround = false;

                                for (int dir = 0; dir < _checkDirections.Length; dir++)
                                {
                                    if (IsInBounds(checkX + _checkDirections[dir].x, checkY + _checkDirections[dir].y) == false)
                                    {
                                        hasRoadAround = true;
                                    } 
                                    else 
                                    {
                                        if (_roadMapHolder.Map[checkX + _checkDirections[dir].x, checkY + _checkDirections[dir].y])
                                        {
                                            hasRoadAround = true;
                                        }
                                    }
                                }

                                if (_roadMapHolder.Map[checkX, checkY] == false && hasRoadAround == false)
                                {
                                    Debug.Log(Vector2Int.Distance(new Vector2Int(checkX, checkY), spawnerPosition).ToString());

                                    if (Vector2Int.Distance(new Vector2Int(checkX, checkY), spawnerPosition) <= _minimalDistanceToSpawner)
                                    {
                                        possibleChestPositions.Add(new Vector2Int(checkX, checkY));
                                    }
                                }
                            }
                        }
                    }
                }

                if (possibleChestPositions.Count > 0)
                {
                    return possibleChestPositions[Random.Range(0, possibleChestPositions.Count)];
                }
                
                radius--;
            }

            return new Vector2Int(0, 0);
        }

        private bool IsInBoundsAndRoadOptional(int x, int y)
        {
            return x >= 0 && x < _islandData.IslandSize && y >= 0 && y < _islandData.IslandSize && (_roadMapHolder.Map[x, y] || optionalMap[x, y]);
        }

        private bool IsInBoundsAndRoad(int x, int y)
        {
            return x >= 0 && x < _islandData.IslandSize && y >= 0 && y < _islandData.IslandSize && _roadMapHolder.Map[x, y];
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < _islandData.IslandSize && y >= 0 && y < _islandData.IslandSize;
        }
    }
}