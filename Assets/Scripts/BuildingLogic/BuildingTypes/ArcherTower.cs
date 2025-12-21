using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class ArcherTower : DefaultCombatTaskConditionProvider
{
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private ProjectileSpeed _projectileSpeed;
    [Cached] private AreaEntityDetector _enemyAreaScaner;

    [SerializeField] private float _arrowLifetime = 5f;
    
    [Header("Links")]
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private Transform _shootingPoint;

    private WeaponPool<Arrow> _arrowObjectPool;

    public readonly OverridableBehaviour<Arrow> ArrowHitBehaviour = new OverridableBehaviour<Arrow>();
    public readonly OverridableBehaviour<Arrow> ArrowHitPositionBehaviour = new OverridableBehaviour<Arrow>();

    private void Start()
    {
        base.Start();
        _arrowObjectPool = new WeaponPool<Arrow>(_arrowPrefab, 3, _ownerEntity,  _arrowLifetime);
        
        foreach (Arrow arrow in _arrowObjectPool.Pool) { SubscribeToArrow(arrow); }
        _arrowObjectPool.PoolObject.ObjectCreated += SubscribeToArrow;
        
        ArrowHitBehaviour.Initialize(_ownerEntity, new DisableArrow());

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Shoot;
    }
    
    private void SubscribeToArrow(Arrow arrow) => arrow.OnArrowHit += OnArrowHit;
    private void OnArrowHit(Arrow arrow) => ArrowHitBehaviour.Execute(arrow);

    private void Shoot()
    {
        Arrow currentArrow = _arrowObjectPool.GetPooledWeapon();

        currentArrow.Launch(_projectileSpeed.Value, _enemyAreaScaner.GetPrioritizedEntity().transform.position, _shootingPoint.position);
    }

    private void OnDestroy() => _arrowObjectPool.DestroyPool();

    private sealed class DisableArrow : CombatBehaviour<Arrow>
    {
        public override void Execute(Arrow arrow)
        {
            arrow.DisableArrow();
        }
    }
}