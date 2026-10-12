using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Zenject;
using System;

namespace Combat.EnemySpawnSystem
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [Inject] private EnemySpawnSystemController _enemySpawnSystem;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        
        [SerializeField] private EnemySpawnerInfoDisplay _enemySpawnerInfoDisplay;
        private float _previousSpawnDelay;

        public EnemySpawnerInfoDisplay SpawnerInfoDisplay => _enemySpawnerInfoDisplay;
        
        private void Awake() => _enemySpawnSystem.AddSpawner(this);
        
        public float GetSpawnDelay(EnemyConfig enemyConfig)
        {
            return Mathf.Min(enemyConfig.SpawnDelay, _previousSpawnDelay) * _waveIndexContainer.GetCurrentWaveContent().SpawnDelayMultiplier;
        }
        
        public async UniTask SpawnEnemyGroup(IReadOnlyList<EnemySpawnData> spawnGroup, CancellationToken cancellationToken)
        {
            foreach (EnemySpawnData enemySpawnData in spawnGroup)
            {
                for (int i = 0; i < enemySpawnData.Amount; i++)
                {
                    SpawnEnemy(enemySpawnData.Config);

                    try
                    {
                        await UniTask.WaitForSeconds(GetSpawnDelay(enemySpawnData.Config), cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        e.LogAsync();
                        
                        return;
                    }
                }
            }
        }
        
        public void SpawnEnemy(EnemyConfig enemyConfig)
        {
            _previousSpawnDelay = enemyConfig.SpawnDelay;
         
            EnemyFactory.Instance.CreateEnemy(enemyConfig, transform.position);
        }
        
        private void OnDestroy() => _enemySpawnSystem.RemoveSpawner(this);
    }
}