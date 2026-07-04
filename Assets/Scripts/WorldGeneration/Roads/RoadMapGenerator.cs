using System.Collections.Generic;
using System.Threading;
using CuroSceneManagement;
using Cysharp.Threading.Tasks;
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

        public async UniTask GenerateRoads()
        {
            await GenerateRoadMap();
            
            AddCenterRoad();
        }

        private async UniTask GenerateRoadMap()
        {
            LoadingScreen.Instance.StartPlayingLoadingIconAnimation().Forget();
            
            bool isComplete = false;
            
            List<Vector2Int> spawnerNodes = _enemyBiomeContainer.GetEnemyBiomesPositions();

            Vector2Int[,] roadNodes = _roadNodeGenerator.GetAllNodes();
            
            _islandData.RoadMapGenerationAlgorithm.SetRandomSeed(Random.Range(int.MinValue, int.MaxValue));
            
            Thread roadGenerationThread = new Thread(() => 
            {
                bool[,] roadMap = _islandData.RoadMapGenerationAlgorithm.GenerateRoadMap(roadNodes, spawnerNodes, _islandData);

                _roadMapHolder.SetRoadMap(roadMap);
            
                isComplete = true;
            });
            
            roadGenerationThread.Start();

            await UniTask.WaitUntil(() => isComplete);
            
            LoadingScreen.Instance.StopPlayingLoadingIconAnimation().Forget();
        }

        private void AddCenterRoad()
        {
            bool[,] roadMap = _roadMapHolder.Map;

            int centerIndex = _islandData.CenterPositionIndex;

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