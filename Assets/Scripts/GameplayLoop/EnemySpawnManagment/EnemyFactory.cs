using UnityEngine;
using Zenject;

namespace Combat
{
    public sealed class EnemyFactory : MonoBehaviour
    {
        private static EnemyFactory _instance;
        
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private DiContainer _container;
        [SerializeField] private EnemyEntity _blankEnemy; 
    
        private ObjectPool<EnemyEntity> _enemyPool;
        
        public static EnemyFactory Instance => _instance;
    
        private void Start()
        {
            _instance = this;
    
            _enemyPool = new ObjectPool<EnemyEntity>(_blankEnemy, 10, _container);
            _container.Inject(_enemyPool);
        }
    
        public EnemyEntity CreateEnemy(EnemyData enemyDataToCreate, Vector3 spawnPosition)
        {
            EnemyEntity newEnemy = _enemyPool.GetNextPooledObject();
            
            newEnemy.transform.position = spawnPosition;
            
            newEnemy.ComponentsContainer.Get<EnemyBootstrap>().SetEnemyData(enemyDataToCreate);
    
            _globalEnemyContainer.Add(newEnemy);
            
            return newEnemy;
        }
    }
}