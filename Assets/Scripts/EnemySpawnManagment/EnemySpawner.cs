using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float _timeBetweenSpawns;
        [SerializeField] private EnemySpawnerInfoDisplayer _enemySpawnerInfoDisplayer;
    
        public Action<EnemyEntity> EnemySpawned;
        public Action<EnemyEntity> EnemyDied;
        public Action LastEnemyKilled;
    
    
        private List<EnemyEntity> _spawnedEnemies;
    
        private List<EnemyData> _enemiesToSpawn;
    
        private YieldInstruction _yieldInstruction;
    
        private void Awake()
        {
            _spawnedEnemies = new();
    
            _yieldInstruction = new WaitForSeconds(_timeBetweenSpawns);
    
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
                StopAllCoroutines();
    
                SpawnEnemy();
    
                StartCoroutine(WaitToSpawn());
            }
        }
    
        public bool AllEnemiesDead() => _spawnedEnemies.Count == 0;
        public bool SpawnedAllEnemies() => _enemiesToSpawn.Count == 0;
    
        public void SpawnGroup()
        {
            _enemySpawnerInfoDisplayer.HideSpawnInfo();
    
            StartCoroutine(WaitToSpawn());
        }
    
        private IEnumerator WaitToSpawn()
        {   
            while (_enemiesToSpawn.Count != 0)
            {  
                yield return new WaitForSeconds(1f);
            
                SpawnEnemy();
            }
        }
    
        private void SpawnEnemy()
        {
            EnemyEntity spawnedEnemy = EnemyFactory.Instance.CreateEnemy(_enemiesToSpawn[0]);
    
            _enemiesToSpawn.RemoveAt(0);
    
            _spawnedEnemies.Add(spawnedEnemy);
    
            spawnedEnemy.transform.position = transform.position;
    
            spawnedEnemy.Health.EnemyDied += RemoveEnemy;
    
            EnemySpawned.Invoke(spawnedEnemy);
        }
    
        private void RemoveEnemy(EnemyEntity enemyEntity)
        {
            _spawnedEnemies.Remove(enemyEntity);
    
            enemyEntity.Health.EnemyDied += RemoveEnemy;
    
            EnemyDied.Invoke(enemyEntity);
    
            if (_spawnedEnemies.Count == 0) LastEnemyKilled?.Invoke();
        }
    
        public void KillAllEnemies() => StartCoroutine(KillEnemies());
        
        private IEnumerator KillEnemies()
        {
            for (int i = 0; i < _spawnedEnemies.Count; i++)
            {
                _spawnedEnemies[i].Health.EnemyDied += RemoveEnemy;
                _spawnedEnemies[i].Health.Die();
    
                yield return new WaitForSeconds(0.25f);
            }        
        }
    }
}