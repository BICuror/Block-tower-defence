using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "NodeRoadAlgorithm", menuName = "Generation/RoadMapGeneratoionAlgorithm/NodeRoadAlgorithm")]

    public sealed class NodeRoadAlgorithm : RoadGenerationAlgorithm
    {
        [SerializeField] private bool _canMoveDiagonnaly;
        [SerializeField] private int _maxIterationsForPathGeneration = 50;
        [Range(0f, 1f)] [SerializeField] private float _chanseToTurnBack = 0.75f;
        [SerializeField] private int _pathRoundness = 1;
        private bool[,] _touchedNodesMap;


        public override bool[,] GenerateRoadMap(Vector2Int[,] roadNodes, List<Vector2Int> spawnerNodes, IslandData islandData)
        {
            _islandData = islandData;

            _roadMap = new bool[_islandData.IslandSize, _islandData.IslandSize];

            _touchedNodesMap = new bool[_islandData.AmountOfRoadNodes, _islandData.AmountOfRoadNodes];

            for (int i = 0; i < spawnerNodes.Count; i++)
            {  
                Vector2Int currentSpawnerPosition = FindSpawnerNodeIndex(spawnerNodes[i], roadNodes, _islandData.AmountOfRoadNodes);

                CreateSpawnerRoad(currentSpawnerPosition, roadNodes);
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

        private void CreateSpawnerRoad(Vector2Int current, Vector2Int[,] roadNodes)
        {
            int middleIndex = (_islandData.AmountOfRoadNodesBetweenCenterAndEdge * 2 + 2) / 2;

            _roadMap[roadNodes[current.x, current.y].x, roadNodes[current.x, current.y].y] = true;

            _touchedNodesMap[current.x, current.y] = true;

            int iterator = 0;
            while (current.x != middleIndex && current.y != middleIndex)
            {
                Vector2Int next = GetNextNodeIndex(middleIndex, current.x, current.y);

                MoveRoad(roadNodes[current.x, current.y], roadNodes[current.x + next.x, current.y + next.y]);

                current.x += next.x;
                current.y += next.y;

                _touchedNodesMap[current.x, current.y] = true;

                if (iterator > _maxIterationsForPathGeneration) 
                {
                    MoveRoad(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]); 
                    current.x = middleIndex;
                    current.y = middleIndex;
                }     
                iterator++;
            }

            MoveRoad(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]);
        }

        private Vector2Int GetNextNodeIndex(int middleIndex, int xIndex, int yIndex)
        {
            int xDifference = NormalizeNumber(middleIndex - xIndex);
            int yDifference = NormalizeNumber(middleIndex - yIndex);

            if (xIndex != 0 && xIndex != _islandData.AmountOfRoadNodes - 1)
            {
                if (xDifference == 0) 
                {
                    if (RandomBool()) xDifference = -1;
                    else xDifference = 1;
                }
                else if (ShouldTurnBack())
                {
                    xDifference *= -1;
                }
            }

            if (yIndex != 0 && yIndex != _islandData.AmountOfRoadNodes - 1)
            {
                if (yDifference == 0)
                {
                    if (RandomBool()) yDifference = -1;
                    else yDifference = 1;
                }
                else if (ShouldTurnBack())
                {
                    yDifference *= -1;
                }
            }
            
            if (NodeIsUnouched(xIndex + xDifference, yIndex + yDifference) && _canMoveDiagonnaly)
            {
                return new Vector2Int(xDifference, yDifference);
            }
            else
            {
                if (RandomBool()) return new Vector2Int(0, yDifference);
                return new Vector2Int(xDifference, 0);
            } 
        }

        private bool ShouldTurnBack() => Random.Range(0f, 1f) < _chanseToTurnBack;
        private bool RandomBool() => Random.Range(0, 100) > 50;
        private bool NodeIsUnouched(int x, int y) => _touchedNodesMap[x, y] == false;

        private void MoveRoad(Vector2Int currentPos, Vector2Int neededPos)
        {
            Vector2Int directionsSummary = new Vector2Int();

            while(currentPos != neededPos)
            {
                Vector2Int moveDirection = GetMoveDirection(directionsSummary, currentPos, neededPos);

                directionsSummary += new Vector2Int(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.y));

                currentPos = currentPos + moveDirection;

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

            if (previousDirectionsSum.x > previousDirectionsSum.y + _pathRoundness) return new Vector2Int(0, yDifference);
            else if (previousDirectionsSum.y > previousDirectionsSum.x + _pathRoundness) return new Vector2Int(xDifference, 0);
            else
            {
                if (RandomBool()) return new Vector2Int(0, yDifference);
                else return new Vector2Int(xDifference, 0);
            }
        }
    }
}