using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class MortarTower : DefaultCombatTaskConditionProvider
{
    [SerializeField] private MortarProjectile projectilePrefab;
    [Cached] private AreaEntityDetector _enemyAreaScaner;
    [Cached] private CombatEntity _ownerEntity;
    private TravelTime _travelTime;
    
    private WeaponPool<MortarProjectile> _grenadeObjectPool; 

    private void Start()
    {   
        base.Start();
        _grenadeObjectPool = new WeaponPool<MortarProjectile>(projectilePrefab, 2, _ownerEntity, 10);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Shoot;
        _travelTime = _ownerEntity.StatContainer.Get<TravelTime>();
    }

    private void Shoot()
    {
        MortarProjectile currentProjectile = _grenadeObjectPool.GetPooledWeapon();
        
        currentProjectile.transform.position = transform.position - new Vector3(0, 0.5f, 0);

        currentProjectile.TravelToPoint(_enemyAreaScaner.RandomItem.transform.position, _travelTime.Value).Forget();
    }

    private void OnDestroy() => _grenadeObjectPool.DestroyPool();
}