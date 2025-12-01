using UnityEngine;
using Cashing;
using Combat;

public sealed class InfernoTower : DefaultCombatTaskConditionProvider
{
    [SerializeField] private WeaponBase _weaponBase;
    [SerializeField] private BeamSystem _beamSystem;
    [SerializeField] private Transform _sourceTransform;
    
    [Cached] private TaskRechargeDuration _taskCycleRechargeDuration;
    [Cached] private AreaEntityDetector _enemyAreaScaner;
    [Cached] private ChargeDuration _chargeDuration;
    [Cached] private CombatEntity _ownerEntity;
    [Cached] private MaxDamage _maxDamage;
    [Cached] private Damage _damage;
    private float _elapsedTime;
    
    private CombatEntity _currentEnemy;
    
    private void Start()
    {
        base.Start();
        
        _weaponBase.Initialize(_ownerEntity);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += Beam;
        _ownerEntity.Draggable.PickedUp += ClearEnemy;
        _enemyAreaScaner.RemovedItem += TryClearEnemy; 
        _beamSystem.SetSource(_sourceTransform);
    }  
    
    private void Beam()
    {
        IncreaseElapsedTime();
        
        if (_currentEnemy == null || _currentEnemy.Health.IsAlive() == false)
        {
            _elapsedTime = 0;
            _currentEnemy = _enemyAreaScaner.GetPrioritizedEntity();
            _beamSystem.SetTarget(_currentEnemy.transform);
        }

        float charge = Mathf.Lerp(0, 1, _elapsedTime / _chargeDuration.Value);
        float damage = _damage.Value * Mathf.Lerp(1, _maxDamage.Value, charge);
        
        _beamSystem.SetAlpha(charge);
        
        _weaponBase.DamageEntity(damage, _currentEnemy);
    }

    private void IncreaseElapsedTime()
    {
        if (_elapsedTime >= _chargeDuration.Value) return;
        
        _elapsedTime += _taskCycleRechargeDuration.Value;
        
        if (_elapsedTime > _chargeDuration.Value) _elapsedTime = _chargeDuration.Value;
    }

    private void TryClearEnemy(CombatEntity removedEnemy)
    {
        if (removedEnemy == _currentEnemy)
        {
            ClearEnemy();
        }
    }
 
    private void ClearEnemy()
    {
        _beamSystem.DisableBeam();
        _currentEnemy = null;
    }
}