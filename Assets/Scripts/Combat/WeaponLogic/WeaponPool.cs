using Combat;

public sealed class WeaponPool<T> where T: Weapon
{
    private ObjectPool<T> _pool;
    private CombatEntity _ownerEntity;
    private float _weaponLifetime;
    
    public WeaponPool(T prefab, int poolSize, CombatEntity ownerEntity, float weaponLifetime, bool useDependencyInjection = false)
    {
        _ownerEntity = ownerEntity; 
        _weaponLifetime = weaponLifetime;
        
        _pool = new ObjectPool<T>(prefab, poolSize, onObjectInitialized: OnPooledObjectCreated);
    }

    public T GetPooledWeapon() => _pool.GetNextPooledObject();
    public void DestroyPool() => _pool.DestroyPool();

    private void OnPooledObjectCreated(T pooledObject)
    {
        pooledObject.Initialize(_ownerEntity, _weaponLifetime);
    }
}