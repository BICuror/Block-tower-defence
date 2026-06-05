using UnityEngine;
using Cashing;
using Combat;

public sealed class EnemyArcherObject : DefaultCombatTaskConditionProvider
{
    [SerializeField] private AreaEntityDetector _enemyAreaScaner;
    [Cached] private ProjectileSpeed _projectileSpeed;
    [Cached] private CombatEntity _ownerEntity;
    
    [SerializeField] private float _arrowLifetime = 5f;
        
    [Header("Links")]
    [SerializeField] private Arrow _arrowPrefab;
    
    private WeaponPool<Arrow> _arrowObjectPool;

    private void Start()
    {
        base.Start();
        _arrowObjectPool = new WeaponPool<Arrow>(_arrowPrefab, 3, _ownerEntity,  _arrowLifetime);
        
        foreach (Arrow arrow in _arrowObjectPool.Pool) { SubscribeToArrow(arrow); }
        _arrowObjectPool.PoolObject.ObjectCreated += SubscribeToArrow;

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Shoot;
    }
    
    private void SubscribeToArrow(Arrow arrow) => arrow.OnArrowHit += OnArrowHit;
    private void OnArrowHit(Arrow arrow) => arrow.DisableArrow();

    private void Shoot()
    {
        Arrow currentArrow = _arrowObjectPool.GetPooledWeapon();

        currentArrow.Launch(_projectileSpeed.Value, _enemyAreaScaner.GetPrioritizedEntity().transform.position, _ownerEntity.transform.position);
    }

    private void OnDestroy() => _arrowObjectPool.DestroyPool();
}