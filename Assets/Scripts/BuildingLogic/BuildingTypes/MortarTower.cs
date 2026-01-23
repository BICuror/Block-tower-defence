using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;
using Combat;

public sealed class MortarTower : DefaultCombatTaskConditionProvider
{
    [SerializeField] private MortarProjectile _projectilePrefab;
    [SerializeField] private ArgumentsContainer _defaultBehaviourArguments;
    [Cached] private AreaEntityDetector _enemyAreaScaner;
    [Cached] private CombatEntity _ownerEntity;
    
    private WeaponPool<MortarProjectile> _grenadeObjectPool;

    public readonly OverridableBehaviour<Vector3> CoreLanded = new();
    public readonly OverridableBehaviour<Transform> CoreLaunched = new();

    private void Start()
    {   
        base.Start();

        CombatBehaviour<Vector3> defaultBehaviour = new ExplosionBehaviour(_defaultBehaviourArguments);
        defaultBehaviour.SetArgumentsContainer(_defaultBehaviourArguments);
        CoreLanded.Initialize(_ownerEntity, defaultBehaviour);
        
        CoreLaunched.Initialize(_ownerEntity);
        
        _grenadeObjectPool = new WeaponPool<MortarProjectile>(_projectilePrefab, 2, _ownerEntity, 10);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += () => Shoot().Forget();
    }

    private async UniTask Shoot()
    {
        MortarProjectile currentProjectile = _grenadeObjectPool.GetPooledWeapon();
        
        currentProjectile.transform.position = transform.position - new Vector3(0, 0.5f, 0);

        CoreLaunched.Execute(currentProjectile.transform);
        
        await currentProjectile.TravelToPoint(_enemyAreaScaner.GetPrioritizedEntity().transform.position);
        
        CoreLanded.Execute(currentProjectile.transform.position);
    }

    private void OnDestroy() => _grenadeObjectPool.DestroyPool();
}