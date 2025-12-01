using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WorldGeneration;
using Zenject;

namespace Combat
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        private const float SPAWN_DELAY = 0.65f;
        
        [Inject] private EnemySpawnSystem _enemySpawnSystem;
        [Inject] private SpawnerRotator _spawnerRotator;
        [SerializeField] private LayerSetting _terrainLayerSetting;
        [SerializeField] private EnemySpawnerInfoDisplayer _enemySpawnerInfoDisplayer;
        private List<EnemyData> _enemiesToSpawn;

        private void Awake()
        {
            UpdateSpawnerHeight();
            _spawnerRotator.RotateSpawner(transform);
            _enemySpawnSystem.AddSpawner(this);
        }

        private void UpdateSpawnerHeight()
        {
            Vector2Int spawnerPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            
            float height = TileMap.GetHitInfo(spawnerPosition, _terrainLayerSetting).point.y;

            if (height == 0) height = 1;
            
            transform.position = new Vector3(transform.position.x, height + 1, transform.position.z);
        }
        
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
        
        private void OnDestroy()
        {
            _enemySpawnSystem.RemoveSpawner(this);
        }
    }
}