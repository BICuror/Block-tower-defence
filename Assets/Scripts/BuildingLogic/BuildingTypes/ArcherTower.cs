using UnityEngine;

public sealed class ArcherTower : CombatBuilding
{
    [Header("Stats")]
    [SerializeField] private float _arrowSpeed;

    [Header("Links")]
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private ApplyEffectContainer _applyEffectContainer;
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;
    [SerializeField] private Transform _shootingPoint;

    private ObjectPool<Arrow> _arrowObjectPool; 

    private void Start()
    {   
        _arrowObjectPool = new ObjectPool<Arrow>(_arrowPrefab, 3);

        TaskCycle buildingTaskCycle = GetComponent<TaskCycle>();

        buildingTaskCycle.ShouldWorkDelegate = ShouldWorkDelegate;

        buildingTaskCycle.TaskPerformed.AddListener(Shoot);
    }

    private bool ShouldWorkDelegate() => _enemyAreaScaner.Empty() == false;

    private void Shoot()
    {   
        Arrow currentArrow = _arrowObjectPool.GetNextPooledObject();
        
        currentArrow.GetRigidbody().velocity = Vector3.zero;
        currentArrow.transform.position = _shootingPoint.position;
        
        currentArrow.transform.LookAt(_enemyAreaScaner.GetFirstEnemy().transform.position);
        
        currentArrow.gameObject.SetActive(true);

        currentArrow.SetEffects(_applyEffectContainer.GetApplyEffects());
        currentArrow.SetContactDamage(Damage);

        currentArrow.GetRigidbody().AddForce(currentArrow.transform.forward * _arrowSpeed, ForceMode.Impulse);
    }

    private void OnDestroy() => _arrowObjectPool.DestroyPool();
}
