using UnityEngine;

public sealed class ArcherTower : MonoBehaviour
{
    [SerializeField] private Rigidbody _arrow;
    
    [SerializeField] private float _arrowSpeed;

    [SerializeField] private int _damage;

    [SerializeField] private ApplyEffectContainer _applyEffectContainer;
    
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    private void Start()
    {   
        TaskCycle buildingTaskCycle = GetComponent<TaskCycle>();

        buildingTaskCycle.ShouldWorkDelegate = ShouldWorkDelegate;

        buildingTaskCycle.TaskPerformed.AddListener(Shoot);

        _applyEffectContainer.ApplyEffectUpdated.AddListener(ModifyArrow);

        ModifyArrow();
    }

    private void ModifyArrow()
    {
        _arrow.GetComponent<Weapon>()._effect = _applyEffectContainer.GetApplyEffects();

        _arrow.GetComponent<Weapon>().SetDamage(_damage);
    }

    private bool ShouldWorkDelegate() => _enemyAreaScaner.Empty() == false;

    private void Shoot()
    {   
        _arrow.velocity = Vector3.zero;
        
        _arrow.gameObject.SetActive(true);

        _arrow.transform.position = transform.position;

        _arrow.transform.LookAt(_enemyAreaScaner.GetFirstEnemy().transform.position);

        _arrow.AddForce(_arrow.transform.forward * _arrowSpeed, ForceMode.Impulse);
    }
}
