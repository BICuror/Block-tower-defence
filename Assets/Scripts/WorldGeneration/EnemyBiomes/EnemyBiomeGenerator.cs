using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private SpawnerRoadNodeGenerator _spawnerRoadNodeGenerator;
        [Inject] private EnemyBiomeContainer _enemyBiomeContainer;
        [Inject] private RoadNodeGenerator _roadNodeGenerator;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private EnemyBiome _enemyBiomePrefab;

        public void TryGenerateNewBiome()
        {
            if (_islandData.MaxAmountOfEnemyBiomes > _enemyBiomeContainer.EnemyBiomeAmount) 
            {
                GenerateNewBiome();
            }
        }

        private void GenerateNewBiome()
        {
            List<Vector2Int> enemyBiomesIndexes = _enemyBiomeContainer.GetEnemyBiomesNodeIndexes();

            Vector2Int spawnerNodeIndex = _spawnerRoadNodeGenerator.GetRandomEnemySpawnerNodeIndex(enemyBiomesIndexes);

            Vector2Int spawnerPosition = _roadNodeGenerator.GetNodePosition(spawnerNodeIndex);

            EnemyBiome biome = _diContainer.InstantiatePrefab(_enemyBiomePrefab.gameObject, new Vector3(spawnerPosition.x, 0f, spawnerPosition.y), Quaternion.identity, null).GetComponent<EnemyBiome>();

            biome.SetCenterPosition(spawnerPosition);    

            biome.SetSpawnerNodeIndex(spawnerNodeIndex);      

            _enemyBiomeContainer.AddBiome(biome);
        }
    }
}