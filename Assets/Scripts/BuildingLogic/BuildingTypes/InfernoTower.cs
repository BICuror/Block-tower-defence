using Combat.IngameUI.CombatEntityCanvas;
using Cysharp.Threading.Tasks;
using Combat.ValueModifiers;
using Combat.Stats;
using UnityEngine;
using Cashing;
using Combat;

public sealed class InfernoTower : DefaultCombatTaskConditionProvider
{
    [SerializeField] private WeaponBase _weaponBase;
    [SerializeField] private BeamSystem _beamSystem;
    [SerializeField] private Transform _sourceTransform;
    [SerializeField] private EntityCanvasBar _chargeBarPrefab;
    [SerializeField] private Sprite _canvasBarIcon;
    
    [Cached] private TaskRechargeDuration _taskCycleRechargeDuration;
    [Cached] private MaxDamageMultiplier _maxDamageMultiplier;
    [Cached] private AreaEntityDetector _enemyAreaScaner;
    [Cached] private ChargeDuration _chargeDuration;
    [Cached] private BuildingEntity _ownerEntity;
    [Cached] private Damage _damage;
    private float _elapsedTime;
    private ChargeableDamageModifier _infernoValueModifier = new();
    
    private EntityCanvasBar _chargeBar;
    private CombatEntity _currentEnemy;
    
    public CombatEntity CurrentTarget => _currentEnemy;
    public OverridableBehaviour OnMaxChargeReached = new();
    public OverridableBehaviour OnEnemyChanged = new();
    
    private void Start()
    {
        base.Start();
        
        _ownerEntity.ValueModifierContainer.DamageDealerContainer.Add(_infernoValueModifier);
        
        _chargeBar = _ownerEntity.ComponentsContainer.Get<EntityCanvas>().AddBar(_canvasBarIcon, 0f, _chargeBarPrefab);
        
        _beamSystem.SetSource(_sourceTransform);
        
        _weaponBase.Initialize(_ownerEntity);

        _ownerEntity.ComponentsContainer.Get<TaskCycle>().TaskPerformed += TryToBeam;
        
        _ownerEntity.Draggable.PickedUp += ClearEnemy;
        _enemyAreaScaner.RemovedItem += TryClearEnemy; 
        
        OnMaxChargeReached.Initialize(_ownerEntity);
        OnEnemyChanged.Initialize(_ownerEntity, new ResetChargeBehaviour());
    }

    public void ResetCharge()
    {
        _elapsedTime = 0f;
        SetCharge(0f);
    }
    
    public bool ResetChargeAndTryFindTarget()
    {
        OnEnemyChanged.Execute();
        
        if (_enemyAreaScaner.IsEmpty) return false;
            
        _currentEnemy = _enemyAreaScaner.GetPrioritizedEntity();
        _beamSystem.SetTarget(_currentEnemy.transform);

        return true;
    }

    public void ChangeChargePercent(float chargeChangeValue)
    {
        _elapsedTime = Mathf.Clamp(_elapsedTime + chargeChangeValue, 0f, _chargeDuration.Value);
        
        if (_elapsedTime >= _chargeDuration.Value) OnMaxChargeReached.Execute();
        
        float charge = _elapsedTime / _chargeDuration.Value;
        
        SetCharge(charge);
    }
    
    private void TryToBeam()
    {
        if (_currentEnemy == null)
        {
            ResetChargeAndTryFindTarget();
            
            return;
        }

        ChangeChargePercent(_taskCycleRechargeDuration.Value);
        
        float charge = _elapsedTime / _chargeDuration.Value;

        SetCharge(charge);
        
        if (_currentEnemy == null)
        {
            ResetChargeAndTryFindTarget();
            
            return;
        }
        
        _weaponBase.DamageEntity(_damage.GetValueModificationContainer(), _currentEnemy);
    }
    
    private void TryClearEnemy(CombatEntity removedEnemy)
    {
        if (removedEnemy == _currentEnemy) ClearEnemy();
    }
 
    private void ClearEnemy()
    {
        OnEnemyChanged.Execute();
        _beamSystem.DisableBeam();
        _currentEnemy = null;
    }

    private void SetCharge(float value)
    {
        _beamSystem.SetAlpha(value);
        _chargeBar.SetValue(value).Forget();
        _infernoValueModifier.SetCharge(value);
    }

    private sealed class ResetChargeBehaviour : CombatBehaviour
    {
        public override void Execute()
        {
            Entity.ComponentsContainer.Get<InfernoTower>().ResetCharge();
        }
    }
}