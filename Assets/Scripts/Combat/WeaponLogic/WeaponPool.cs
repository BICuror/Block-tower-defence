using System.Collections.Generic;
using Combat;

public sealed class WeaponPool<T> where T : WeaponBase
{
    private ObjectPool<T> _pool;
    private CombatEntity _ownerEntity;
    private float _weaponLifetime;

    private List<WeaponPoolModifier> _weaponModifiers = new();
    
    public ObjectPool<T> Pool => _pool;
    
    public WeaponPool(T prefab, int poolSize, CombatEntity ownerEntity, float weaponLifetime = 0, bool useDependencyInjection = false)
    {
        _ownerEntity = ownerEntity; 
        _weaponLifetime = weaponLifetime;
        
        _pool = new ObjectPool<T>(prefab, poolSize, onObjectInitialized: OnPooledObjectCreated);
    }

    public T GetPooledWeapon() => _pool.GetNextPooledObject();
    public void DestroyPool() => _pool.DestroyPool();

    private void OnPooledObjectCreated(T pooledObject)
    {
        if (_weaponLifetime != 0)
        {
            (pooledObject as Weapon).Initialize(_ownerEntity, _weaponLifetime);
        }
        else
        {
            pooledObject.Initialize(_ownerEntity);
        }
        
        _weaponModifiers.ForEach(modifier => modifier.AddWeaponModification(pooledObject));
    }

    public void AddWeaponModifier(WeaponPoolModifier modifier)
    {
        _weaponModifiers.Add(modifier);

        IReadOnlyList<T> poolContent = _pool.Pool;

        for (int i = 0; i < poolContent.Count; i++)
        {
            modifier.AddWeaponModification(poolContent[i]);
        }
    }

    public void RemoveWeaponModifier(WeaponPoolModifier modifier)
    {
        _weaponModifiers.Remove(modifier);

        IReadOnlyList<T> poolContent = _pool.Pool;

        for (int i = 0; i < poolContent.Count; i++)
        {
            modifier.RemoveWeaponModification(poolContent[i]);
        }
    }
}

public abstract class WeaponPoolModifier
{
    public abstract void AddWeaponModification(WeaponBase weapon);
    public abstract void RemoveWeaponModification(WeaponBase weapon);
}