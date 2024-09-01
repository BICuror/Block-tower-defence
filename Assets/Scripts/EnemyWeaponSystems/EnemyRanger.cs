using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyRanger : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileDamage;
    
    [SerializeField] private List<Effect> _effectsToApply;

    [Header("Links")]
    [SerializeField] private BuildingHealthAreaScaner _buildingHealthScaner;
    [SerializeField] private EnemyWeapon _projectilePrefab;
    [SerializeField] private Transform _shootingPoint;

    private EnemyTaskCycle _enemyTaskCycle;

    private void Start()
    {
        _enemyTaskCycle = GetComponent<EnemyTaskCycle>();

        _enemyTaskCycle.ShouldWorkDelegate = ShouldWork; 
        _enemyTaskCycle.TaskPerformed.AddListener(Shoot);

        _buildingHealthScaner.PlacedComponentAdded.AddListener(ActivateCycle);
    
        GetComponent<SpecialEnemyObject>().GetEnemyHealth().DeathEvent.AddListener(DisableShootingAbility);
    }

    private void ActivateCycle(Building building) => _enemyTaskCycle.StartCycle();

    private bool ShouldWork() => _buildingHealthScaner.HasPlacedComponents();

    private void Shoot()
    {  
        EnemyWeapon currentProjectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
        
        currentProjectile.transform.LookAt(_buildingHealthScaner.GetFirstPlacedEntity().transform.position);

        currentProjectile.SetEffects(_effectsToApply);
        currentProjectile.SetContactDamage(_projectileDamage);
        currentProjectile.SetTargetBuilding(_buildingHealthScaner.GetFirstHealthComponent() as BuildingHealth);

        currentProjectile.GetRigidbody().AddForce(currentProjectile.transform.forward * _projectileSpeed, ForceMode.Impulse);
    }

    private void DisableShootingAbility(GameObject enemy)
    {
        Destroy(_enemyTaskCycle);
        Destroy(this);
    }
}
