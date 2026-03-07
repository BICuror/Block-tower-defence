using UnityEngine;
using Cashing;
using Combat;
using Cysharp.Threading.Tasks;

public sealed class LightningTower : DefaultCombatTaskConditionProvider
{
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private AreaEntityDetector _enemyAreaScaner;

    [Header("Links")] 
    [SerializeField] private PropogationStrike _ligningPrefab;
    [SerializeField] private Transform _shootingPoint;

    private WeaponPool<PropogationStrike> _lightningStrikeObjectPool;

    private void Start()
    {
        base.Start();
        _lightningStrikeObjectPool = new WeaponPool<PropogationStrike>(_ligningPrefab, 3, _ownerEntity);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Shoot;
    }

    private void Shoot()
    {
        PropogationStrike ligning = _lightningStrikeObjectPool.GetPooledWeapon();

        ligning.StartPropogationStrike(_enemyAreaScaner.GetPrioritizedEntity(), _shootingPoint.position).Forget();
    }

    private void OnDestroy() => _lightningStrikeObjectPool.DestroyPool();
}