using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Navigation;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const float SPAWN_DELAY = 0.65f;
        
        [SerializeField] private EnemySpawnerInfoDisplayer _enemySpawnerInfoDisplayer;
        private List<EnemyEntity> _spawnedEnemies; 
        private List<EnemyData> _enemiesToSpawn;
        
        public Action<EnemyEntity> EnemySpawned;
        public Action<EnemyEntity> EnemyDied;
        public Action LastEnemyKilled;
        
        private void Awake()
        {
            _spawnedEnemies = new();
    
            LastEnemyKilled += TryToSpawnEnemy;
        }
    
        public void SetEnemiesToSpawn(List<EnemyData> enemiesToSpawn)
        {
            _enemiesToSpawn = enemiesToSpawn;
    
            _enemySpawnerInfoDisplayer.DisplaySpawnInfo(_enemiesToSpawn);
        }
    
        private void TryToSpawnEnemy() 
        {
            if (AllEnemiesDead() && SpawnedAllEnemies() == false) 
            {
                SpawnEnemy();
            }
        }
    
        public bool AllEnemiesDead() => _spawnedEnemies.Count == 0;
        public bool SpawnedAllEnemies() => _enemiesToSpawn.Count == 0;

        public async UniTask SpawnGroup()
        {
            _enemySpawnerInfoDisplayer.HideSpawnInfo();
            
            while (_enemiesToSpawn.Count > 0)
            {
                SpawnEnemy();
                
                await UniTask.WaitForSeconds(SPAWN_DELAY);
            }
        }
        
        private void SpawnEnemy()
        {
            EnemyEntity spawnedEnemy = EnemyFactory.Instance.CreateEnemy(_enemiesToSpawn[0]);
    
            _enemiesToSpawn.RemoveAt(0);
    
            _spawnedEnemies.Add(spawnedEnemy);
    
            spawnedEnemy.transform.position = transform.position;
    
            spawnedEnemy.EnemyHealth.EnemyDied += RemoveEnemy;
            spawnedEnemy.ComponentsContainer.Get<NavigationAgent>().Initialize();
    
            EnemySpawned.Invoke(spawnedEnemy);
        }
    
        private void RemoveEnemy(EnemyEntity enemyEntity)
        {
            _spawnedEnemies.Remove(enemyEntity);
    
            enemyEntity.EnemyHealth.EnemyDied -= RemoveEnemy;
    
            EnemyDied?.Invoke(enemyEntity);
    
            if (_spawnedEnemies.Count == 0) LastEnemyKilled?.Invoke();
        }
    }
}