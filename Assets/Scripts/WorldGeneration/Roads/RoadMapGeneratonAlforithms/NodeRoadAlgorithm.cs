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
        
        public override bool TryGenerateRoadMap()
        {
            _touchedNodesMap = new bool[IslandData.AmountOfRoadNodes, IslandData.AmountOfRoadNodes];

            return CreateSpawnerRoad();
        }
        
        private bool CreateSpawnerRoad()
        {
            Vector2Int currentNodeIndex = GetClosestNodeIndex(StartPosition);
            
            ConnectPoints(StartPosition, RoadNodes[currentNodeIndex.x, currentNodeIndex.y]);
            
            while (current.x != middleIndex && current.y != middleIndex)
            {
                Vector2Int next = GetNextNodeIndex(middleIndex, current.x, current.y);

                ConnectPoints(roadNodes[current.x, current.y], roadNodes[current.x + next.x, current.y + next.y]);

                current.x += next.x;
                current.y += next.y;

                _touchedNodesMap[current.x, current.y] = true;

                if (iterator > _maxIterationsForPathGeneration) 
                {
                    ConnectPoints(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]); 
                    current.x = middleIndex;
                    current.y = middleIndex;
                }     
                iterator++;
            }

            ConnectPoints(roadNodes[current.x, current.y], roadNodes[middleIndex, middleIndex]);
        }

        private Vector2Int GetClosestNodeIndex(Vector2Int position)
        {
            Vector2Int bestVectorIndex = Vector2Int.zero;
            float bestDistance = float.MaxValue;
            
            for (int x = 0; x < RoadNodes.GetLength(0); x++)
            {
                for (int z = 0; z < RoadNodes.GetLength(1); z++)
                {
                    float distance = Vector2Int.Distance(position, new Vector2Int(x, z));

                    if (bestDistance > distance)
                    {
                        bestDistance = distance;
                        bestVectorIndex = new Vector2Int(x, z);
                    }
                }
            }
            
            return bestVectorIndex;
        }
        
        private Vector2Int GetNextNodeIndex(int middleIndex, int xIndex, int yIndex)
        {
            int xDifference = NormalizeNumber(middleIndex - xIndex);
            int yDifference = NormalizeNumber(middleIndex - yIndex);

            if (xIndex != 0 && xIndex != IslandData.AmountOfRoadNodes - 1)
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

            if (yIndex != 0 && yIndex != IslandData.AmountOfRoadNodes - 1)
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
    }
}