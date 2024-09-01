using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeContainer
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        private List<EnemyBiome> _enemyBiomes = new List<EnemyBiome>();
        public IReadOnlyList<EnemyBiome> EnemyBiomeList => _enemyBiomes;

        public int EnemyBiomeAmount => _enemyBiomes.Count;

        public void AddBiome(EnemyBiome biomeToAdd)
        {
            _enemyBiomes.Add(biomeToAdd);
        }
        public void DisableBiomesTerrain(float duration)
        {
            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                _enemyBiomes[i].DisableTerrain(duration);    
            }
        }

        public void EnableBiomesTerrain(float duration)
        {
            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                _enemyBiomes[i].EnableTerrain(duration);    
            }
        }

        public void GenerateBiomesDecorations()
        {
            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                _enemyBiomes[i].GenerateDecorations();    
            }
        }

        public void IncreaseBiomesStages()
        {
            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                _enemyBiomes[i].IncreaseCurrentStage();
            }
        }

        public void DestroyOldBiomes()
        {
            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                if (_enemyBiomes[i].GetStage() >= _islandData.EnemyBiomeStages.Length)
                {
                    _enemyBiomes[i].Destroy();

                    _enemyBiomes.RemoveAt(i);
                }
            }
        }

        public void RegenerateBiomes()
        {
            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                _enemyBiomes[i].RegenerateBiome();
            }
        }

        public List<Vector2Int> GetEnemyBiomesNodeIndexes()
        {
            List<Vector2Int> result = new List<Vector2Int>();

            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                result.Add(_enemyBiomes[i].SpawnerNodeIndex);
            }
            
            return result;
        }

        public List<Vector2Int> GetEnemyBiomesPositions()
        {
            List<Vector2Int> result = new List<Vector2Int>();

            for (int i = 0; i < _enemyBiomes.Count; i++)
            {
                result.Add(_enemyBiomes[i].GetCenterPosition());
            }
            
            return result;
        }
    }
}