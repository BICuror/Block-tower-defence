using System.Collections.Generic;
using System;
using Combat;

public sealed class WeaponPool<T> where T : WeaponBase
{
    private ObjectPool<T> _pool;
    private CombatEntity _ownerEntity;
    private float _weaponLifetime;
    
    public IReadOnlyList<T> Pool => _pool.Pool;
    public ObjectPool<T> PoolObject => _pool;
    
    public WeaponPool(T prefab, int poolSize, CombatEntity ownerEntity, float weaponLifetime = 0, Predicate<T> isFreeElement = null)
    {
        _ownerEntity = ownerEntity; 
        _weaponLifetime = weaponLifetime;
        
        _pool = new ObjectPool<T>(prefab, poolSize, isFreeElement: isFreeElement);

        foreach (T weapon in _pool.Pool) { InitializeWeapon(weapon); }
        _pool.ObjectCreated += InitializeWeapon;
    }

    private void InitializeWeapon(T weapon)
    {
        if (_weaponLifetime == 0) weapon.Initialize(_ownerEntity);
        else (weapon as Weapon).Initialize(_ownerEntity, _weaponLifetime);
    }

    public T GetPooledWeapon() => _pool.GetNextPooledObject();
    public void DestroyPool() => _pool.DestroyPool();
}