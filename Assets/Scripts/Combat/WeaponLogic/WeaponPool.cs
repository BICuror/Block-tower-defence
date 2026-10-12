using System.Collections.Generic;
using System;
using Combat;

public sealed class WeaponPool<T> where T : WeaponBase
{
    private ObjectPool<T> _pool;
    private CombatEntity _ownerEntity;
    private float _weaponLifetime;
    
    public IReadOnlyList<T> Pool => _pool.Pool;
    
    private Action<T> _onWeaponCreated;
    
    public WeaponPool(T prefab, int poolSize, CombatEntity ownerEntity, float weaponLifetime = 0, Predicate<T> isFreeElement = null)
    {
        _ownerEntity = ownerEntity; 
        _weaponLifetime = weaponLifetime;
        
        _pool = new ObjectPool<T>(prefab, poolSize, isFreeElement: isFreeElement);

        _pool.SetObjectCreatedDelegate(InitializeWeapon);
    }
    
    public void SetWeaponCreatedDelegate(Action<T> onWeaponCreated)
    {
        if (_onWeaponCreated != null) throw new Exception("OnWeaponCreated is already set");
        
        _onWeaponCreated = onWeaponCreated;

        foreach (T poolObject in Pool)
        {
            _onWeaponCreated.Invoke(poolObject);
        }
    }

    private void InitializeWeapon(T weapon)
    {
        if (_weaponLifetime == 0) weapon.Initialize(_ownerEntity);
        else (weapon as Weapon).Initialize(_ownerEntity, _weaponLifetime);
    }

    public T GetPooledWeapon() => _pool.GetNextPooledObject();
    public void DestroyPool() => _pool.DestroyPool();
}