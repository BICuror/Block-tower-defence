using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeContainer
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private List<EnemyBiome> _enemyBiomes = new();
        
        private IslandData _islandData => _islandDataContainer.Data;
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
            for (int i = _enemyBiomes.Count - 1; i >= 0; i--)
            {
                if (_enemyBiomes[i].CurrentStage >= _islandData.EnemyBiomeStages.Length)
                {
                    DestroyBiome(_enemyBiomes[i]);
                }
            }
        }

        public void DestroyExcessiveBiomes(int maxAmount)
        {
            List<EnemyBiome> sortedBiomes = _enemyBiomes.OrderBy(biome => biome.CurrentStage).ToList();

            for (int i = maxAmount; i < sortedBiomes.Count; i++)
            {
                DestroyBiome(sortedBiomes[i]);
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

        private void DestroyBiome(EnemyBiome biome)
        {
            biome.Destroy();

            _enemyBiomes.Remove(biome);
        }
    }
}