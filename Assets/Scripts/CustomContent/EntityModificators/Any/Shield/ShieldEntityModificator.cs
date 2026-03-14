using Zenject;
using System;
using Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ShieldEntityModificator : EntityModificator
{
    [Inject] private WaveStateMachine _waveStateMachine;
    private EntityCanvasIcon _shieldIcon;
    private EntityCanvasBar _shieldBar;
    private ShieldDamageModificator _shieldDamageModificator;
    
    public override void Enable()
    {
        InitializeShieldDamageModificator();
        _shieldDamageModificator.ShieldHealthUpdated += OnShieldHealthUpdated;
        
        Entity.DamageModifierContainer.ReciverContainer.Add(_shieldDamageModificator);

        Initialize();

        _waveStateMachine.StateStarted += TryReinitializeShield;
    }

    private void InitializeShieldDamageModificator()
    {
        float shieldHealth = 0f;

        if (Args.HasArgument("MaxShieldHealth"))
        {
            shieldHealth += Args.GetArgument<float>("MaxShieldHealth");
        }

        if (Args.HasArgument("ShieldHealthScale"))
        {
            shieldHealth += Args.GetArgument<float>("ShieldHealthScale") * Entity.Health.GetMaxHp();
        }
        
        _shieldDamageModificator = new(shieldHealth);
    }

    private void TryReinitializeShield(WaveState waveState)
    {
        if (waveState == WaveState.Idle) Initialize();
    }

    private void Initialize()
    {
        if (!_shieldIcon) _shieldIcon = AddIcon(false);
        
        _shieldDamageModificator.ResetShieldHealth();
    }

    #region UI

    private void OnShieldHealthUpdated(float shieldHealthPercent)
    {
        TryRemoveIcon(shieldHealthPercent);
        TryAddBar(shieldHealthPercent);
        TryRemoveBar(shieldHealthPercent);
        UpdateShieldBar(shieldHealthPercent);
    }
    
    private void TryRemoveIcon(float shieldHealthPercent)
    {
        if (!_shieldIcon || shieldHealthPercent == 1f) return;
        
        RemoveIcon(_shieldIcon);
    }

    private void TryRemoveBar(float shieldHealthPercent)
    {
        if (!_shieldBar || shieldHealthPercent != 0f) return;
  
        RemoveBar(_shieldBar);
    }

    private void TryAddBar(float shieldHealthPercent)
    {
        if (_shieldBar || shieldHealthPercent == 1f) return;

        _shieldBar = AddBar(shieldHealthPercent, Args.GetArgument<GameObject>("CanvasBarPrefab").GetComponent<EntityCanvasBar>());
    }

    private void UpdateShieldBar(float shieldHealthPercent)
    {
        if (!_shieldBar) return;
        
        _shieldBar.SetValue(shieldHealthPercent).Forget();
    }

    #endregion
    
    public override void Disable()
    {
        TryRemoveIcon(0f);
        TryRemoveBar(0f);
        Entity.DamageModifierContainer.ReciverContainer.Remove(_shieldDamageModificator);
        _shieldDamageModificator.ShieldHealthUpdated -= OnShieldHealthUpdated;
        _waveStateMachine.StateStarted -= TryReinitializeShield;
    }

    private sealed class ShieldDamageModificator : DamageModifier
    {
        private float _currentShieldHealth;
        private float _maxShieldHealth;
        
        public override ResolveOrder Order => ResolveOrder.Final;

        public event Action<float> ShieldHealthUpdated; 
        
        public ShieldDamageModificator(float maxShieldHealth)
        {
            _currentShieldHealth = maxShieldHealth;
            _maxShieldHealth = maxShieldHealth;
        }

        public void ResetShieldHealth()
        {
            _currentShieldHealth = _maxShieldHealth;
            ShieldHealthUpdated?.Invoke(1f);
        }
        
        public override float Modify(CombatEntity otherEntity, float value)
        {
            if (_currentShieldHealth > 0f)
            {
                _currentShieldHealth -= value;
                
                if (_currentShieldHealth <= 0f) _currentShieldHealth = 0f;
                
                ShieldHealthUpdated?.Invoke(_currentShieldHealth / _maxShieldHealth);

                DamageNumberDisplayManager.Instance.DisplayDamageNumber(value, OwnerEntity.transform.position, DamageType.Shielded);
                
                return 0f;
            }
            
            return value;
        }
    }
}
