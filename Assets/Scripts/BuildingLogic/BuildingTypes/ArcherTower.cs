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

    public WeaponPool<Arrow> WeaponPool => _arrowObjectPool;
    public readonly OverridableBehaviour<Arrow> ArrowHitBehaviour = new OverridableBehaviour<Arrow>();

    private void Start()
    {
        base.Start();
        _arrowObjectPool = new WeaponPool<Arrow>(_arrowPrefab, 3, _ownerEntity, _arrowLifetime);
        
        foreach (Arrow arrow in _arrowObjectPool.Pool.Pool) { SubscribeToArrow(arrow); }
        _arrowObjectPool.Pool.ObjectCreated += SubscribeToArrow;
        
        ArrowHitBehaviour.Initialize(_ownerEntity, new DisableArrow());

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Shoot;
    }
    
    private void SubscribeToArrow(Arrow arrow) => arrow.OnArrowHit += OnArrowHit;
    private void OnArrowHit(Arrow arrow) => ArrowHitBehaviour.Execute(arrow);

    private void Shoot()
    {
        Arrow currentArrow = _arrowObjectPool.GetPooledWeapon();

        currentArrow.RB.velocity = Vector3.zero;
        currentArrow.transform.position = _shootingPoint.position;

        currentArrow.transform.LookAt(_enemyAreaScaner.FirstItem.transform.position);

        currentArrow.RB.AddForce(currentArrow.transform.forward * _projectileSpeed.Value, ForceMode.Impulse);
    }

    private void OnDestroy() => _arrowObjectPool.DestroyPool();

    private sealed class DisableArrow : CombatBehaviour<Arrow>
    {
        public override void Execute(Arrow arrow)
        {
            arrow.DisableArrow().Forget();
        }
    }
}