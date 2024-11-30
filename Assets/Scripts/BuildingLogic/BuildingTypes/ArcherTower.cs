using Cashing;
using Combat;
using UnityEngine;

public sealed class ArcherTower : DefaultCombatTaskConditionProvider
{
    [Cached] private EnemyAreaScaner _enemyAreaScaner;
    [Cached] private Damage _damage;
    
    [Header("Stats")]
    [SerializeField] private float _arrowSpeed;

    [Header("Links")]
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private Transform _shootingPoint;

    private ObjectPool<Arrow> _arrowObjectPool;

    private void Start()
    {
        base.Start();
        _arrowObjectPool = new ObjectPool<Arrow>(_arrowPrefab, 3);

        TaskCycle buildingTaskCycle = GetComponent<TaskCycle>();

        buildingTaskCycle.TaskPerformed += Shoot;
    }

    private void Shoot()
    {
        Arrow currentArrow = _arrowObjectPool.GetNextPooledObject();

        currentArrow.GetRigidbody().velocity = Vector3.zero;
        currentArrow.transform.position = _shootingPoint.position;

        currentArrow.transform.LookAt(_enemyAreaScaner.FirstItem.transform.position);

        currentArrow.gameObject.SetActive(true);
        currentArrow.SetContactDamage(_damage.Value);

        currentArrow.GetRigidbody().AddForce(currentArrow.transform.forward * _arrowSpeed, ForceMode.Impulse);
    }

    private void OnDestroy() => _arrowObjectPool.DestroyPool();
}