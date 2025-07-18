using Combat;

public class WeaponBasePool<T> where T : WeaponBase
{
    private ObjectPool<T> _pool;
    private CombatEntity _ownerEntity;
    
    public ObjectPool<T> Pool => _pool;
    
    public WeaponBasePool(T prefab, int poolSize, CombatEntity ownerEntity, bool useDependencyInjection = false)
    {
        _ownerEntity = ownerEntity; 
        
        _pool = new ObjectPool<T>(prefab, poolSize, onObjectInitialized: OnPooledObjectCreated);
    }

    public T GetPooledWeapon() => _pool.GetNextPooledObject();
    
    public void DestroyPool() => _pool.DestroyPool();

    private void OnPooledObjectCreated(T pooledObject)
    {
        pooledObject.Initialize(_ownerEntity);
    }
}
