using Combat.ValueModifiers;
using Combat.Stats;
using Cashing;
using System;

namespace Combat
{
    public abstract class EntityHealth
    {
        [Cached] private CombatEntity _entity;
        [Cached] private MaxHealth _maxHpStat;
        
        private readonly TokenContainer _invulnerabilityTokenContainer = new(false);
        private float _maxHealth;
        private float _currentHp;
        
        public TokenContainer InvulnerabilityTokenContainer => _invulnerabilityTokenContainer;
        
        public event Action Damaged;
        public event Action Healed;
        public event Action Died;
        public event Action HandleDeath;
        public event Action<float> DamageReceived;
        public event Action<CombatEntity> EntityDamaged;
        public event Action<CombatEntity> EntityDied; 
        
        public void Initialize()
        {
            _maxHealth = _maxHpStat.Value;
            _maxHpStat.ValueChanged += _ => ClampCurrentHpByMax();
        }
        public void RefilHP() => ReceiveHeal(_maxHealth);
        public float GetMaxHp() => _maxHealth;
        public float GetHp() => _currentHp;
        public float GetHpPercent() => _currentHp / _maxHealth;
        public bool IsAlive() => _currentHp > 0f;
        public bool IsFullHp() => _currentHp == _maxHealth;

        private void ClampCurrentHpByMax()
        {
            float healthPercent = GetHpPercent();

            _maxHealth = _maxHpStat.Value;

            _currentHp = _maxHealth * healthPercent;
        }
        
        #region DamageRecivement 
        public void ReceiveEffectDamage(float damage, DamageVisualsType damageVisualsType)
        {
            if (!IsAlive() || !_invulnerabilityTokenContainer.IsEmpty) return;
            
            ReceiveDamage(damage, damageVisualsType);
            OnDamageTaken(damage);
        }

        public void ReceiveEnemyDamage(ValueModificationContainer valueModificationContainer, CombatEntity damageDealer)
        {
            if (!IsAlive() || !_invulnerabilityTokenContainer.IsEmpty) return;
            
            if (damageDealer.ValueModifierContainer.DamageDealerContainer.TryModify(valueModificationContainer, _entity)) return;
            
            if (_entity.ValueModifierContainer.DamageReceiverContainer.TryModify(valueModificationContainer, damageDealer)) return;
            
            if (!IsAlive()) return;

            float resultDamage = valueModificationContainer.GetModifiedValue();
            
            ReceiveDamage(resultDamage, DamageVisualsType.Damage);
            
            damageDealer.ValueModifierContainer.InvokeOnEnemyHit(_entity);
            damageDealer.ValueModifierContainer.InvokeOnDamageDealt(resultDamage);
            
            if (!IsAlive())
            {
                damageDealer.ValueModifierContainer.InvokeOnKillEffects(_entity);
                _entity.ValueModifierContainer.InvokeOnDeathEffects(damageDealer);
            }
            
            OnDamageTaken(resultDamage);
        }

        private void ReceiveDamage(float damage, DamageVisualsType damageVisualsType)
        {
            _currentHp -= damage;

            DamageNumberDisplayManager.Instance.DisplayDamageNumber(damage, _entity.transform.position, damageVisualsType);
        }

        private void OnDamageTaken(float damage)
        {
            if (!IsAlive()) Die();
            else
            {
                EntityDamaged?.Invoke(_entity);
                Damaged?.Invoke();
                DamageReceived?.Invoke(damage);
            }
        }
        #endregion
        
        #region HealRecivement
        public void ReceivePercentHeal(float percent) => ReceiveHeal(percent * _maxHealth);
        
        public void ReceiveHeal(ValueModificationContainer valueModificationContainer, CombatEntity healDealer)
        {
            if (healDealer.ValueModifierContainer.HealDealerContainer.TryModify(valueModificationContainer, _entity)) return;
            
            if (_entity.ValueModifierContainer.HealReceiverContainer.TryModify(valueModificationContainer, healDealer)) return;
            
            float resultHeal = valueModificationContainer.GetModifiedValue();
            
            ReceiveHeal(resultHeal);
        }

        private void ReceiveHeal(float healValue)
        {
            if (healValue <= 0 || IsFullHp()) return;
            
            if (_currentHp + healValue < _maxHealth) _currentHp += healValue;
            else _currentHp = _maxHealth;
            
            Healed?.Invoke();
        }
        
        #endregion
        
        public void Die()
        {
            _currentHp = 0;
            Died?.Invoke();
            EntityDied?.Invoke(_entity);
            InvokeEntityDied();
            HandleDeath.Invoke();
        }

        protected abstract void InvokeEntityDied();
    }
}