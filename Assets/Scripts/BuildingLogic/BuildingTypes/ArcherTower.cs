using UnityEngine;
using Cashing;
using Combat;

public sealed class ArcherTower : DefaultCombatTaskConditionProvider
{
    [Cached] private CombatEntity _ownerEntity;
    private EnemyAreaScaner _enemyAreaScaner;
    private ProjectileSpeed _projectileSpeed;

    [SerializeField] private float _arrowLifetime = 5f;
    
    [Header("Links")]
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private Transform _shootingPoint;

    private WeaponPool<Arrow> _arrowObjectPool;

    private void Start()
    {
        base.Start();
        _arrowObjectPool = new WeaponPool<Arrow>(_arrowPrefab, 3, _ownerEntity, _arrowLifetime);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Shoot;
        _enemyAreaScaner = _ownerEntity.ComponentsContainer.Get<EnemyAreaScaner>();
        _projectileSpeed = _ownerEntity.StatContainer.Get<ProjectileSpeed>();
    }

    private void Shoot()
    {
        Arrow currentArrow = _arrowObjectPool.GetPooledWeapon();

        currentArrow.RB.velocity = Vector3.zero;
        currentArrow.transform.position = _shootingPoint.position;

        currentArrow.transform.LookAt(_enemyAreaScaner.FirstItem.transform.position);

        currentArrow.RB.AddForce(currentArrow.transform.forward * _projectileSpeed.Value, ForceMode.Impulse);
    }

    private void OnDestroy() => _arrowObjectPool.DestroyPool();
}