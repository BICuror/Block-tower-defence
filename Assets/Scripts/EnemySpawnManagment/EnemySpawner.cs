using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace Combat
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawnerInfoDisplayer _enemySpawnerInfoDisplayer;
        private CancellationTokenSource _cancellationTokenSource = new();
        private List<EnemyData> _enemiesToSpawn;
        private float _previousSpawnDelay;
        
        public bool SpawnedAllEnemies => _enemiesToSpawn.Count == 0;
        public int EntitiesAmountToSpawn => _enemiesToSpawn.Count;
        public List<EnemyData> EnemiesToSpawn => _enemiesToSpawn;
        
        public void SetEnemiesToSpawn(List<EnemyData> enemiesToSpawn)
        {
            _enemiesToSpawn = enemiesToSpawn;
    
            _enemySpawnerInfoDisplayer.SetSpawnInfo(_enemiesToSpawn);
        }

        public void ShowEnemySpawnInfo() => _enemySpawnerInfoDisplayer.ShowSpawnInfo();
        public void HideEnemySpawnInfo() => _enemySpawnerInfoDisplayer.HideSpawnInfo();
        
        public void TrySpawnEnemy()
        {
            if (_enemiesToSpawn.Count > 0) SpawnEnemy();
        }
        
        public async UniTask SpawnGroup()
        {
            HideEnemySpawnInfo();
            
            while (true)
            {
                TrySpawnEnemy();
                
                if (_enemiesToSpawn.Count > 0)
                {
                    float minimalSpawnDelay = Mathf.Min(_enemiesToSpawn[0].SpawnDelay, _previousSpawnDelay);

                    try
                    {
                        await UniTask.WaitForSeconds(minimalSpawnDelay, cancellationToken: _cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        e.LogAsync();
                        break;
                    }
                }
                else break;
            }
        }

        public void StopSpawning()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new();
        }
        
        private void SpawnEnemy()
        {
            _previousSpawnDelay = _enemiesToSpawn[0].SpawnDelay;
         
            EnemyFactory.Instance.CreateEnemy(_enemiesToSpawn[0], transform.position);
            
            _enemiesToSpawn.RemoveAt(0);
        }
    }
}