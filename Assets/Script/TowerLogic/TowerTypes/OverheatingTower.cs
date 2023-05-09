using UnityEngine;

public sealed class OverheatingTower : MonoBehaviour
{
    [SerializeField] private GameObject _arrow;
    
    [SerializeField] private float _arrowSpeed;

    [SerializeField] private int _damage;

    [SerializeField] private ApplyEffectContainer _applyEffectContainer;
    
    [SerializeField] private EnemyAreaScaner _enemyAreaScaner;

    [SerializeField] private OverheatManager _overheatingManager;

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

    public bool ShouldWorkDelegate() => _enemyAreaScaner.Empty() == false &&  _overheatingManager.IsOverheated() == false;

    public void Shoot()
    {
        _overheatingManager.AddOverheat(20f);

        _arrow.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        _arrow.SetActive(true);

        _arrow.transform.position = transform.position;

        _arrow.transform.LookAt(_enemyAreaScaner.GetFirstEnemy().transform.position);

        _arrow.GetComponent<Rigidbody>().AddForce(_arrow.transform.forward * _arrowSpeed, ForceMode.Impulse);
    }
}
