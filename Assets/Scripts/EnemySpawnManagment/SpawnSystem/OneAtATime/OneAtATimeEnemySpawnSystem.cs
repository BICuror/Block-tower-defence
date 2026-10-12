using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Combat.EnemySpawnSystem.OneAtATime
{
    public sealed class OneAtATimeEnemySpawnSystem : MonoBehaviour, IEnemySpawnSystem
    {
        [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private GlobalStatContainer _globalStatContainer;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        private bool _allEnemiesSpawned;
        
        private Dictionary<EnemySpawner, List<EnemySpawnOrder>> _spawnProgresses;

        public bool AllEnemiesSpawned => _allEnemiesSpawned;
        
        public void PrepareWave(WaveContent waveContent, List<EnemySpawner> spawners) {}
        
        public async UniTask SpawnWave(WaveContent waveContent, List<EnemySpawner> spawners)
        {
            _allEnemiesSpawned = false;
            
            PopulateSpawnProgress(spawners);
            
            EnemySpawner previousSpawner = spawners[Random.Range(0, spawners.Count)];
            
            while (_spawnProgresses.Count > 0)
            {
                List<EnemySpawner> remainingSpawners = _spawnProgresses.Keys.ToList();

                if (remainingSpawners.Count > 1) remainingSpawners.Remove(previousSpawner);
                
                EnemySpawner nextSpawner = remainingSpawners[Random.Range(0, remainingSpawners.Count)];

                await SpawnEnemiesForDuration(nextSpawner, waveContent.TimeBeforeSwitchingToOtherSpawner);
                
                previousSpawner = nextSpawner;
            }

            _allEnemiesSpawned = true;
            
            await UniTask.WaitUntil(() => _globalEnemyContainer.Entities.Count == 0);
        }

        private void PopulateSpawnProgress(List<EnemySpawner> spawners)
        {
            _spawnProgresses = new();
            
            foreach (EnemySpawner spawner in spawners)
            {
                List<EnemySpawnOrder> spawnOrders = new();
                
                foreach (EnemySpawnData spawnData in _enemySpawnGroupContainer.GetEnemySpawnDataForSpawner(spawner))
                {
                    spawnOrders.Add(new EnemySpawnOrder(spawnData.Config, spawnData.Amount));
                }
                
                _spawnProgresses.Add(spawner, spawnOrders);
            }
        }
        
        private async UniTask SpawnEnemiesForDuration(EnemySpawner spawner, float duration)
        {
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                EnemyConfig enemyConfig = _spawnProgresses[spawner][0].Config;
                
                float spawnDelay = spawner.GetSpawnDelay(enemyConfig);
                
                spawner.SpawnEnemy(enemyConfig);
                
                await UniTask.WaitForSeconds(spawnDelay);
                
                elapsedTime += spawnDelay;

                DecreaseSpawnOrderCounter(spawner);
                
                if (!_spawnProgresses.ContainsKey(spawner)) return;
            }
        }

        private void DecreaseSpawnOrderCounter(EnemySpawner spawner)
        {
            _spawnProgresses[spawner][0].LeftToSpawn--;
                
            if (_spawnProgresses[spawner][0].LeftToSpawn <= 0) _spawnProgresses[spawner].RemoveAt(0);
            
            if (_spawnProgresses[spawner].Count == 0) _spawnProgresses.Remove(spawner);
        }
        
        private class EnemySpawnOrder
        {
            public EnemyConfig Config;
            public int LeftToSpawn;
            
            public EnemySpawnOrder(EnemyConfig config, int leftToSpawn)
            {
                Config = config;
                LeftToSpawn = leftToSpawn;
            }
        }
    }
}