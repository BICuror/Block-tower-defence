using UnityEngine;
using Cashing;
using Combat;

public sealed class InfernoTower : DefaultCombatTaskConditionProvider
{
    [SerializeField] private WeaponBase _weaponBase;
    [SerializeField] private BeamSystem _beamSystem;
    [SerializeField] private Transform _sourceTransform;
    
    [Cached] private TaskRechargeDuration _taskCycleRechargeDuration;
    [Cached] private MaxDamageMultiplier _maxDamageMultiplier;
    [Cached] private AreaEntityDetector _enemyAreaScaner;
    [Cached] private ChargeDuration _chargeDuration;
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private Damage _damage;
    private float _elapsedTime;
    
    private CombatEntity _currentEnemy;
    
    public CombatEntity CurrentTarget => _currentEnemy;
    public OverridableBehaviour OnMaxChargeReached = new();
    
    private void Start()
    {
        base.Start();
        _beamSystem.SetSource(_sourceTransform);
        
        _weaponBase.Initialize(_ownerEntity);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += TryToBeam;
        
        _ownerEntity.Draggable.PickedUp += ClearEnemy;
        _enemyAreaScaner.RemovedItem += TryClearEnemy; 
        
        OnMaxChargeReached.Initialize(_ownerEntity);
    }  
    
    public bool ResetChargeAndTryFindTarget()
    {
        _elapsedTime = 0f;
        
        if (_enemyAreaScaner.IsEmpty) return false;
            
        _currentEnemy = _enemyAreaScaner.GetPrioritizedEntity();
        _beamSystem.SetTarget(_currentEnemy.transform);

        return true;
    }
    
    private void TryToBeam()
    {
        if (_currentEnemy == null)
        {
            if (!ResetChargeAndTryFindTarget()) return;
        }
        else
        { 
            IncreaseElapsedTime();

            if (_currentEnemy == null)
            {
                if (!ResetChargeAndTryFindTarget()) return;
            }
        }
        
        float charge = _elapsedTime / _chargeDuration.Value;
        float damage = _damage.Value * Mathf.Lerp(1, _maxDamageMultiplier.Value, charge);
        
        _beamSystem.SetAlpha(charge);
        
        _weaponBase.DamageEntity(damage, _currentEnemy);
    }

    private void IncreaseElapsedTime()
    {
        if (_elapsedTime >= _chargeDuration.Value) return;
        
        _elapsedTime += _taskCycleRechargeDuration.Value;

        if (_elapsedTime >= _chargeDuration.Value)
        {
            _elapsedTime = _chargeDuration.Value;
            
            OnMaxChargeReached.Execute();
        }
    }
    
    private void TryClearEnemy(CombatEntity removedEnemy)
    {
        if (removedEnemy == _currentEnemy) ClearEnemy();
    }
 
    private void ClearEnemy()
    {
        _beamSystem.DisableBeam();
        _currentEnemy = null;
    }
}