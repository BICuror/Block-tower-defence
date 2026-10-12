using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Combat.EnemySpawnSystem.Default
{
    public sealed class DefaultEnemySpawnSystem : MonoBehaviour, IEnemySpawnSystem
    {
        [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        private bool _allEnemiesSpawned;

        public bool AllEnemiesSpawned => _allEnemiesSpawned;

        public void PrepareWave(WaveContent waveContent, List<EnemySpawner> spawners) {}

        public async UniTask SpawnWave(WaveContent waveContent, List<EnemySpawner> spawners)
        {
            _allEnemiesSpawned = false;
            
            List<UniTask> spawnTasks = new();
            
            foreach (EnemySpawner enemySpawner in spawners)
            {
                spawnTasks.Add(SpawnEnemiesFromSpawner(enemySpawner));
            }
            
            await UniTask.WhenAll(spawnTasks);

            _allEnemiesSpawned = true;
            
            await UniTask.WaitUntil(() => _globalEnemyContainer.Entities.Count == 0);
        }

        private async UniTask SpawnEnemiesFromSpawner(EnemySpawner spawner)
        {
            List<EnemySpawnData> enemySpawnData = _enemySpawnGroupContainer.GetEnemySpawnDataForSpawner(spawner);
            
            await spawner.SpawnEnemyGroup(enemySpawnData, CancellationToken.None);
        }
    }
}