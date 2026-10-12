using Combat.Enemies;
using UnityEngine;
using Zenject;

namespace Combat.EnemySpawnSystem
{
    public sealed class EnemyFactory : MonoBehaviour
    {
        private static EnemyFactory _instance;
        
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [SerializeField] private EnemyEntity _blankEnemy; 
    
        private ObjectPool<EnemyEntity> _enemyPool;
        
        public static EnemyFactory Instance => _instance;
    
        private void Start()
        {
            _instance = this;
    
            _enemyPool = new ObjectPool<EnemyEntity>(_blankEnemy, 10);
        }
    
        public EnemyEntity CreateEnemy(EnemyConfig enemyConfigToCreate, Vector3 spawnPosition)
        {
            EnemyEntity newEnemy = _enemyPool.GetNextPooledObject();
            
            newEnemy.transform.position = spawnPosition;
            
            newEnemy.ComponentsContainer.Get<EnemyBootstrap>().SetEnemyConfig(enemyConfigToCreate);
    
            _globalEnemyContainer.Add(newEnemy);
            
            return newEnemy;
        }
    }
}