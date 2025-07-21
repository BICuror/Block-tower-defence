using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class RoadMapGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
        [Inject] private RoadNodeGenerator _roadNodeGenerator;
        [Inject] private RoadMapHolder _roadMapHolder;
        
        private IslandData _islandData => _islandDataContainer.Data;

        public void GenerateRoads()
        {
            List<Vector2Int> spawnerNodes = _enemyBiomeContainer.GetEnemyBiomesPositions();

            Vector2Int[,] roadNodes = _roadNodeGenerator.GetAllNodes();

            bool[,] roadMap = _islandData.RoadMapGenerationAlgorithm.GenerateRoadMap(roadNodes, spawnerNodes, _islandData);
            
            _roadMapHolder.SetRoadMap(roadMap);

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
    }
}