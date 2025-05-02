using System.Collections.Generic;
using Cashing;
using UnityEngine;
using Combat;

public sealed class InfernoTower : DefaultCombatTaskConditionProvider
{
    [Cached] private EnemyAreaScaner _enemyAreaScaner;
    
    private int _fullAccerationTicks;
    private float _elapsedTicks;

    [SerializeField] private BeamSystem _beamSystem;

    private EnemyHealth _currentEnemy;
    
    /*private void Start()
    {
        base.Start();
        
        _buildingTaskCycle = GetComponent<BuildingTaskCycle>();
        _buildingTaskCycle.ShouldWorkDelegate = ShouldWork;
        _buildingTaskCycle.TaskPerformed.AddListener(Beam);

        _fullAccerationTicks = Mathf.RoundToInt(_damageAccselerationTime / _buildingTaskCycle.RechargeTime);

        _building = GetComponent<Building>();
        _building.PickedUp.AddListener(ClearEnemy);
        _building.BuildCompleted.AddListener(SetRandomNewEnemy);

        _enemyAreaScaner.EnemyEnteredArea.AddListener(TrySetNewEnemy);
        _enemyAreaScaner.EnemyExitedArea.AddListener(TryRemoveEnemy);
    }  

    private void ClearEnemy()
    {
        _beamSystem.DisableBeam();
        _currentEnemy = null;
    }
    
    private void SetRandomNewEnemy()
    {
        if (ShouldWork())
        {
            SetNewEnemy(_enemyAreaScaner.GetFirstEnemy());
        }
    }

    private void TrySetNewEnemy(EnemyHealth enemy)
    {
        if (_currentEnemy == null && _building.IsBuilt())
        {
            SetNewEnemy(enemy);
        }
    }

    private void SetNewEnemy(EnemyHealth enemyHealth)
    {
        if (_currentEnemy == null)
        {
            _beamSystem.SetTarget(enemyHealth.transform);

            _currentEnemy = enemyHealth;

            _elapsedTicks = 0f;
        }
    }

    private void TryRemoveEnemy(EnemyHealth enemy)
    {
        if (_currentEnemy == enemy)
        {
            ClearEnemy();

            if (_enemyAreaScaner.Empty() == false) 
            {
                SetNewEnemy(_enemyAreaScaner.GetFirstEnemy());
            }
            else 
            {
                _buildingTaskCycle.StopCycle();
            }
        }
    }

    private void Beam()
    {
        _elapsedTicks += 1;
        _elapsedTicks = Mathf.Min(_elapsedTicks, _fullAccerationTicks);

        float evaluatedTime = _elapsedTicks / _fullAccerationTicks;
        
        _beamSystem.SetAlpha(evaluatedTime);

        _currentEnemy.GetHurt(Mathf.Lerp(Damage, _maxDamage, _damageCurve.Evaluate(evaluatedTime)));

        if (_currentEnemy != null && _currentEnemy.IsAlive()) ApplyEffectsToEnemy();
    }*/
}
