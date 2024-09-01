using UnityEngine;

public sealed class MortarTower : CombatBuilding
{
    [Header("StatSettings")]
    [Range(0f, 1f)] [SerializeField] private float _explotionDamageMultiplyer;
    [SerializeField] private float _explotionRadius;
    public float ExplotionDamage => Damage * _explotionDamageMultiplyer;

    [Header("Links")]
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;
    [SerializeField] private MortarGrenade _grenadePrefab;
    [SerializeField] private ApplyEffectContainer _applyEffectContainer;

    private ObjectPool<MortarGrenade> _grenadeObjectPool; 

    private void Start()
    {   
        _grenadeObjectPool = new ObjectPool<MortarGrenade>(_grenadePrefab, 2);

        TaskCycle buildingTaskCycle = GetComponent<TaskCycle>();

        buildingTaskCycle.ShouldWorkDelegate = ShouldWorkDelegate;

        buildingTaskCycle.TaskPerformed.AddListener(Shoot);
    }

    private bool ShouldWorkDelegate() => _enemyAreaScaner.Empty() == false;

    private void Shoot()
    {
        MortarGrenade currentGrenade = _grenadeObjectPool.GetNextPooledObject();
        
        currentGrenade.transform.position = transform.position;

        currentGrenade.SetContactDamage(Damage);
        currentGrenade.SetExplotionDamage(Damage * _explotionDamageMultiplyer);
        currentGrenade.SetExplotionRaduis(_explotionRadius);

        currentGrenade.SetEffects(_applyEffectContainer.GetApplyEffects());

        currentGrenade.Launch(_enemyAreaScaner.GetRandomEnemy().transform.position);
    }

    private void OnDestroy() => _grenadeObjectPool.DestroyPool();
}
