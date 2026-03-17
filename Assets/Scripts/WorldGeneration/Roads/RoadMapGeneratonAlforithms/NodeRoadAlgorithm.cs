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
        
        private void CreateSpawnerRoad(Vector2Int current, Vector2Int[,] roadNodes)
        {
            int middleIndex = (_islandData.AmountOfRoadNodesBetweenCenterAndEdge * 2 + 2) / 2;

            _roadMap[roadNodes[current.x, current.y].x, roadNodes[current.x, current.y].y] = true;

            _touchedNodesMap[current.x, current.y] = true;

            int iterator = 0;
            while (current.x != middleIndex && current.y != middleIndex)
            {
                Vector2Int next = GetNextNodeIndex(middleIndex, current.x, current.y);

                MoveRoadTo(roadNodes[current.x, current.y], roadNodes[current.x + next.x, current.y + next.y]);

                current.x += next.x;
                current.y += next.y;

                _touchedNodesMap[current.x, current.y] = true;

                if (iterator > _maxIterationsForPathGeneration) 
                {
                    MoveRoadTo(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]); 
                    current.x = middleIndex;
                    current.y = middleIndex;
                }     
                iterator++;
            }

            MoveRoadTo(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]);
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

        private bool ShouldTurnBack() => Random(0, 100) < _chanseToTurnBack * 100;
        private bool RandomBool() => Random(0, 100) > 50;
        private bool NodeIsUnouched(int x, int y) => _touchedNodesMap[x, y] == false;
    }
}