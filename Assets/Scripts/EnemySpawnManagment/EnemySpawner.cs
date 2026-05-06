using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Combat
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpawnerInfoDisplayer _enemySpawnerInfoDisplayer;
        private List<EnemyData> _enemiesToSpawn;
        
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

        public async UniTask SpawnGroup()
        {
            _enemySpawnerInfoDisplayer.HideSpawnInfo();
            
            while (true)
            {
                float minimalSpawnDelay = _enemiesToSpawn[0].SpawnDelay;
                
                SpawnEnemy();
                
                if (_enemiesToSpawn.Count > 0)
                {
                    minimalSpawnDelay = Mathf.Min(_enemiesToSpawn[0].SpawnDelay, minimalSpawnDelay);
                    
                    await UniTask.WaitForSeconds(minimalSpawnDelay);
                }
                else return;
            }
        }
        
        private void SpawnEnemy()
        {
            EnemyEntity spawnedEnemy = EnemyFactory.Instance.CreateEnemy(_enemiesToSpawn[0], transform.position);
    
            _enemiesToSpawn.RemoveAt(0);
        }
    }
}