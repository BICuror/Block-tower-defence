using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const float SPAWN_DELAY = 0.65f;
        
        [SerializeField] private EnemySpawnerInfoDisplayer _enemySpawnerInfoDisplayer;
        private List<EnemyData> _enemiesToSpawn;
        
        public void SetEnemiesToSpawn(List<EnemyData> enemiesToSpawn)
        {
            _enemiesToSpawn = enemiesToSpawn;
    
            _enemySpawnerInfoDisplayer.DisplaySpawnInfo(_enemiesToSpawn);
        }
    
        private void TryToSpawnEnemy() 
        {
            if (SpawnedAllEnemies() == false) 
            {
                SpawnEnemy();
            }
        }
    
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
            EnemyEntity spawnedEnemy = EnemyFactory.Instance.CreateEnemy(_enemiesToSpawn[0], transform.position);
    
            _enemiesToSpawn.RemoveAt(0);
        }
    }
}