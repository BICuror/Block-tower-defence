using UnityEngine;

public sealed class EnemyFactory : MonoBehaviour
{
    private static EnemyFactory _instance;
    public static EnemyFactory Instance => _instance;
    
    [SerializeField] private EnemyBootstrap _blankEnemy; 

    private ObjectPool<EnemyBootstrap> _enemyPool;

    private void Start()
    {
        _instance = this;

        _enemyPool = new ObjectPool<EnemyBootstrap>(_blankEnemy, 10);
    }

    public EnemyHealth CreateEnemy(EnemyData _enemyDataToCreate)
    {
        EnemyBootstrap newEnemy = _enemyPool.GetNextPooledObject();

        newEnemy.SetEnemyData(_enemyDataToCreate);

        return newEnemy.Health;
    }
}
