using UnityEngine;

namespace Combat
{
    public sealed class EnemyFactory : MonoBehaviour
    {
        private static EnemyFactory _instance;
        public static EnemyFactory Instance => _instance;
        
        [SerializeField] private EnemyEntity _blankEnemy; 
    
        private ObjectPool<EnemyEntity> _enemyPool;
    
        private void Start()
        {
            _instance = this;
    
            _enemyPool = new ObjectPool<EnemyEntity>(_blankEnemy, 10);
        }
    
        public EnemyEntity CreateEnemy(EnemyData _enemyDataToCreate)
        {
            EnemyEntity newEnemy = _enemyPool.GetNextPooledObject();
    
            newEnemy.ComponentsContainer.Get<EnemyBootstrap>().SetEnemyData(_enemyDataToCreate);
    
            return newEnemy;
        }
    }
}